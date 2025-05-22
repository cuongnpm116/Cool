using Cool.Application.Dtos.Files;

namespace Cool.Application.Interfaces.Services;

public interface IFileService
{
    Task<List<string>> GetFilesName(Guid tenantId);
    Task<bool> AddFile(CreateFileModel newFile);
}
