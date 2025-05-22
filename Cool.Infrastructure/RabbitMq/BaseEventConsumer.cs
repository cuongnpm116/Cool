using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Cool.Infrastructure.RabbitMq;

public abstract class BaseEventConsumer<T> : BackgroundService, IDistributedEvent<T>
{
    protected readonly IModel Channel;
    protected readonly IConnection Connection;
    public bool WaitUntilDone => true;
    public abstract DeclareOption DeclareOption { get; }
    private static readonly ActivitySource ActivitySource = new(Assembly.GetEntryAssembly().GetName().Name);
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private readonly ILogger<BaseEventConsumer<T>> _logger;
    public BasicDeliverEventArgs Args { get; set; }

    public BaseEventConsumer(
        IRabbitMQConnector connector,
        ILogger<BaseEventConsumer<T>> logger)
    {
        Connection = connector.Connection;
        Channel = connector.Connection.CreateModel();
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Channel.BasicQos(DeclareOption.QoS.PrefetchSize, DeclareOption.QoS.PrefetchCount, DeclareOption.QoS.Global);
        Channel.QueueDeclare(DeclareOption.Queue.Name, DeclareOption.Queue.Durable, DeclareOption.Queue.Exclusive, DeclareOption.Queue.AutoDelete, DeclareOption.Queue.Arguments);

        foreach (DeclareOption.BindingOption item in DeclareOption.Binding)
        {
            Channel.ExchangeDeclare(item.Exchange.Name, item.Exchange.Type, item.Exchange.Durable, item.Exchange.AutoDelete, item.Exchange.Arguments);
            Channel.QueueBind(DeclareOption.Queue.Name, item.Exchange.Name, item.RoutingKey, item.Arguments);
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Channel.Close();
        Channel.Dispose();
        Connection.Close();
        Connection.Dispose();
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.Received += async (sender, args) =>
        {
            T data = default;

            try
            {
                // Extract the PropagationContext of the upstream parent from the message headers.
                PropagationContext parentContext = Propagator.Extract(default, args.BasicProperties, this.ExtractTraceContextFromBasicProperties);
                Baggage.Current = parentContext.Baggage;

                // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
                // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#span-name
                using Activity activity = ActivitySource.StartActivity("ReceiveMessage", ActivityKind.Consumer, parentContext.ActivityContext);

                string message = Encoding.UTF8.GetString(args.Body.ToArray());
                activity?.SetTag("messaging.message", message);

                // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
                // See:
                //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#messaging-attributes
                //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.rabbitmq.queue", DeclareOption.Queue.Name);
                activity?.SetTag("messaging.rabbitmq.exhcnage", args.Exchange);
                activity?.SetTag("messaging.rabbitmq.routing_key", args.RoutingKey);
                activity?.SetTag("messaging.rabbitmq.delivery_tag", args.DeliveryTag);

                if (typeof(T) == typeof(string))
                {
                    data = (T)Convert.ChangeType(message, typeof(T));
                }
                else
                {
                    data = JsonConvert.DeserializeObject<T>(message);
                }

                Args = args;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            await HandleAsync(data);
        };
        Channel.BasicConsume(DeclareOption.Queue.Name, DeclareOption.AutoAck, consumer);
        return Task.CompletedTask;
    }

    public void Ack(bool multiple = false)
    {
        Channel.BasicAck(deliveryTag: Args.DeliveryTag, multiple);
    }

    private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
    {
        try
        {
            if (props.Headers.TryGetValue(key, out object value))
            {
                byte[] bytes = value as byte[];
                return new[] { Encoding.UTF8.GetString(bytes) };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract trace context.");
        }

        return Enumerable.Empty<string>();
    }

    public abstract Task HandleAsync(T data);
}