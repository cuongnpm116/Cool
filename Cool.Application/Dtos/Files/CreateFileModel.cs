namespace Cool.Application.Dtos.Files;

public class CreateFileModel
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public Guid TenantId { get; set; }
}
