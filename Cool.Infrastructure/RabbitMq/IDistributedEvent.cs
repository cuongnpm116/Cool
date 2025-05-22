namespace Cool.Infrastructure.RabbitMq;

public interface IDistributedEvent
{
    bool WaitUntilDone { get; }
}

public interface IDistributedEvent<T> : IDistributedEvent
{
    Task HandleAsync(T data);
}
