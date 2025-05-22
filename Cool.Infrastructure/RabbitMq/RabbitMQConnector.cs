using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Cool.Infrastructure.RabbitMq;

public interface IRabbitMQConnector
{
    IConnection Connection { get; }
}

public class RabbitMQConnector : IRabbitMQConnector
{
    public IConnection Connection { get; private set; }

    private readonly RabbitMQOption _options;

    public RabbitMQConnector(IOptions<RabbitMQOption> options)
    {
        _options = options.Value;
        ConnectionFactory factory = new ConnectionFactory()
        {
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        List<AmqpTcpEndpoint> hosts = _options.Hosts.Select(x => new AmqpTcpEndpoint(x.HostName, x.Port)).ToList();
        Connection = factory.CreateConnection(hosts);
    }
}