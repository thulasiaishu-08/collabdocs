using DocEditor.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DocEditor.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentShare> DocumentShares => Set<DocumentShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Password).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<Document>(e =>
        {
            e.Property(d => d.Title).HasMaxLength(200).IsRequired();
            e.HasOne(d => d.Owner)
             .WithMany(u => u.Documents)
             .HasForeignKey(d => d.OwnerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentShare>(e =>
        {
            e.HasIndex(s => new { s.DocumentId, s.SharedWithUserId }).IsUnique();
            e.HasOne(s => s.Document)
             .WithMany(d => d.Shares)
             .HasForeignKey(s => s.DocumentId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.SharedWithUser)
             .WithMany(u => u.SharedWithMe)
             .HasForeignKey(s => s.SharedWithUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
