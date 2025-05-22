using Cool.Application.Dtos.Users;
using Cool.Application.Interfaces.Repositories;
using Cool.Application.Interfaces.Services;
using Cool.Domain.Entities;
using Jarvis.Application.Interfaces.Repositories;

namespace Cool.Persistence.Services;

public class UserService : IUserService
{
    private readonly IMasterUnitOfWork _masterUoW;

    public UserService(IMasterUnitOfWork masterUoW)
    {
        _masterUoW = masterUoW;
    }

    public async Task<int> CreateUser(CreateUserModel model)
    {
        Guid tenantId = model.TenantId;

        // Nếu chưa có tenant thì tạo mới
        if (tenantId == Guid.Empty)
        {
            var newTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = model.TenantName,
                ConnectionString = $"Host=127.0.0.1;Port=5432;Database=Cool_File;Username=Postgres;Password=@bc19xyZ"
            };

            var tenantRepo = _masterUoW.GetRepository<IRepository<Tenant>>();
            await tenantRepo.InsertAsync(newTenant);
            int tenantSaveResult = await _masterUoW.CommitAsync();
            if (tenantSaveResult <= 0)
            {
                throw new Exception("Error while creating new tenant");
            }

            tenantId = newTenant.Id;
        }

        // Tạo user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Email = model.Email,
            Password = model.Password,
            TenantId = tenantId
        };

        var userRepo = _masterUoW.GetRepository<IRepository<User>>();
        await userRepo.InsertAsync(user);
        int userSaveResult = await _masterUoW.CommitAsync();
        if (userSaveResult <= 0)
        {
            throw new Exception("Error while creating new user");
        }

        return userSaveResult;
    }
}
