using Cool.Application.Dtos.Files;
using Cool.Application.Interfaces.Repositories;
using Cool.Application.Interfaces.Services;
using Jarvis.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cool.Persistence.Services;

public class FileService : IFileService
{
    private readonly IAppUnitOfWork _appUoW;

    public FileService(IAppUnitOfWork appUoW)
    {
        _appUoW = appUoW;
    }

    public async Task<List<string>> GetFilesName(Guid tenantId)
    {
        var fileRepo = _appUoW.GetRepository<IRepository<Domain.Entities.File>>();
        List<string> fileNames = await fileRepo.GetQuery()
            .Where(f => f.TenantId == tenantId)
            .Select(f => f.Name).ToListAsync();

        return fileNames;
    }

    public async Task<bool> AddFile(CreateFileModel newFileModel)
    {
        Domain.Entities.File newFile = new()
        {
            Id = Guid.NewGuid(),
            Name = newFileModel.Name,
            Extension = newFileModel.Extension,
            TenantId = newFileModel.TenantId
        };
        var fileRepo = _appUoW.GetRepository<IRepository<Domain.Entities.File>>();
        await fileRepo.InsertAsync(newFile);
        int saveResult = await _appUoW.CommitAsync();
        return saveResult > 0;
    }
}
