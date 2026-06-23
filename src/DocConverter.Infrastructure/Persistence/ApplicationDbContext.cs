using System.Reflection;
using DocConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocConverter.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<ConversionJob> ConversionJobs => Set<ConversionJob>();
    public DbSet<ConversionJobSourceFile> ConversionJobSourceFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}