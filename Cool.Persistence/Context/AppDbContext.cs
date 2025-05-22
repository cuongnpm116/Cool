using Jarvis.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cool.Persistence.Context;

public class AppDbContext : DbContext, IStorageContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.File>(builder =>
        {
            builder.ToTable("files");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).IsRequired();
        });
    }
}
