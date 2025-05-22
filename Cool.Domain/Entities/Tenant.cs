using Jarvis.Domain.Common.Interfaces;

namespace Cool.Domain.Entities;

public class Tenant : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}
