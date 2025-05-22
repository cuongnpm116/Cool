using Cool.Domain.Entities;
using Jarvis.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cool.Persistence.Context;

public class MasterDbContext : DbContext, IStorageContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.ToTable("tenants");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).IsRequired();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).IsRequired();
        });
    }
}
