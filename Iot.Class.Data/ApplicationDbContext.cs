using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Iot.Class.Domain.ReadModels;
namespace Iot.Class.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public virtual DbSet<ClassReadModel> Classes { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ClassReadModel>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.ToTable("Class");
        });
    }
}
