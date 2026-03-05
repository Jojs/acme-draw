using Acme.Draw.Core;
using Microsoft.EntityFrameworkCore;

namespace Acme.Draw.Web.Data;

public class DrawDbContext : DbContext
{
    public DrawDbContext(DbContextOptions<DrawDbContext> options) : base(options)
    {
    }

    public DbSet<StoredSubmission> Submissions => Set<StoredSubmission>();

    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoredSubmission>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(320);

            entity.Property(x => x.SerialNumber).IsRequired().HasMaxLength(100);

            entity.Property(x => x.CreatedAtUtc).IsRequired();

            entity.HasIndex(x => x.SerialNumber);
        });

        modelBuilder.Entity<SerialNumber>(entity =>
        {
            entity.HasKey(x => x.Value);

            entity.Property(x => x.Value)
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}