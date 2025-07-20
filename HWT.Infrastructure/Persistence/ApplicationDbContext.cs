using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using HWT.Domain.Entities;

namespace HWT.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kill> Kills { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Kill>(entity =>
        {
            entity.HasKey(k => k.Id);
            entity.HasOne(k => k.User)
                .WithMany()
                .HasForeignKey(k => k.UserId);
            entity.HasIndex(k => k.Timestamp);
            entity.HasIndex(k => new { k.UserId, k.Timestamp });
        });
    }
}