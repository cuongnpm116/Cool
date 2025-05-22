using Cool.Application.Interfaces.Repositories;
using Cool.Domain.Entities;
using Jarvis.Application.Interfaces.Repositories;
using Jarvis.Application.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cool.Persistence.Resolver;

public class HostTenantIdResolver : ITenantIdResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HostTenantIdResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        string hostname = _httpContextAccessor.HttpContext.Request.Host.Value;
        var uow = _httpContextAccessor.HttpContext.RequestServices.GetService<IMasterUnitOfWork>();
        var repo = uow.GetRepository<IRepository<Tenant>>();
        Tenant tenant = repo.GetQuery().FirstOrDefault(x => x.Name == hostname);
        if (tenant == null)
        {
            return Guid.Empty;
        }
        return tenant.Id;
    }
}
