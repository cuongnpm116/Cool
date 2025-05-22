using Jarvis.Domain.Common.Interfaces;

namespace Cool.Domain.Entities;

public class User : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public Guid TenantId { get; set; }
}
