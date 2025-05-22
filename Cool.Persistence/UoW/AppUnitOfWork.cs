using Cool.Application.Interfaces.Repositories;
using Cool.Persistence.Context;
using Jarvis.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cool.Persistence.UoW;

public class AppUnitOfWork : BaseEFUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    public AppUnitOfWork(IServiceProvider services, IDbContextFactory<AppDbContext> factory)
        : base(services, factory)
    {
    }
}
