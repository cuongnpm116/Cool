using Cool.Application.Interfaces.Repositories;
using Cool.Domain.Entities;
using Jarvis.Application.Interfaces.Repositories;
using Jarvis.Application.MultiTenancy;
using Jarvis.Persistence.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cool.Persistence.Resolver;

public class HttpStorageConnectionStringResolver : ITenantConnectionStringResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpStorageConnectionStringResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetConnectionString(string tenantIdOrName)
    {
        Guid tenantId = GetTenantId(tenantIdOrName);
        var masterUoW = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IMasterUnitOfWork>();
        var tenantRepo = masterUoW.GetRepository<IRepository<Tenant>>();
        Tenant tenant = tenantRepo.GetQuery().FirstOrDefault(x => x.Id == tenantId);
        return tenant is null
            ? throw new Exception($"Connection string of tenant {tenantId} not found")
            : tenant.ConnectionString;
    }

    private Guid GetTenantId(string tenantIdOrName)
    {
        if (!string.IsNullOrEmpty(tenantIdOrName))
        {
            if (Guid.TryParse(tenantIdOrName, out var parsedId))
            {
                return parsedId;
            }

            parsedId = Guid.Empty;
        }

        var httpContext = _httpContextAccessor.HttpContext ?? throw new Exception("HttpContext is not available.");

        ITenantIdResolver resolver = null;

        var services = httpContext.RequestServices.GetServices<ITenantIdResolver>();

        if (!string.IsNullOrEmpty(httpContext.Request.Headers["X-Tenant-Id"]))
        {
            resolver = services.FirstOrDefault(x => x is HeaderTenantIdResolver);
        }
        else if (httpContext.Request.Query.TryGetValue("tenantId", out _))
        {
            resolver = services.FirstOrDefault(x => x is QueryTenantIdResolver);
        }
        else
        {
            resolver = services.FirstOrDefault(x => x is HostTenantIdResolver);
        }

        if (resolver == null)
        {
            throw new Exception("Cannot resolve ITenantIdResolver.");
        }

        return resolver.GetTenantId();
    }
}

