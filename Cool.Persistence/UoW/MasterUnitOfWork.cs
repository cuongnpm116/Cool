using Cool.Application.Interfaces.Repositories;
using Cool.Persistence.Context;
using Jarvis.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cool.Persistence.UoW;

public class MasterUnitOfWork : BaseEFUnitOfWork<MasterDbContext>, IMasterUnitOfWork
{
    public MasterUnitOfWork(IServiceProvider services, IDbContextFactory<MasterDbContext> factory)
        : base(services, factory)
    {
    }
}
