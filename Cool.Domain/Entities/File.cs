using Jarvis.Domain.Common;

namespace Cool.Domain.Entities;

public class File : BaseFullAuditableEntity<Guid>
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }

    public string StoragePath { get; set; } = string.Empty;
    public string StorageProvider { get; set; } = string.Empty;

    public Guid TenantId { get; set; }
}
