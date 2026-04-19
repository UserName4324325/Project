using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebBankApplication.Models;

namespace WebBankApplication.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Remittance> Remittances { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        modelBuilder.Entity<Remittance>(entity =>
        {
            entity.HasOne(r => r.Sender)
                .WithMany(u => u.SentRemittances)
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Recipient)
                .WithMany(u => u.ReceivedRemittances)
                .HasForeignKey(r => r.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }

}
