using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HWT.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HWT.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DiscordGuildMember> DiscordGuildMembers { get; set; }
        public DbSet<DiscordRole> DiscordRoles { get; set; }
        public DbSet<GameLogEvent> GameLogEvents { get; set; }
        public DbSet<Kill> KillEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create the value converter for List<string> to JSON
            var rolesConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );

            var rolesComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            // DiscordRole: Use string as PK
            builder.Entity<DiscordRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            // DiscordGuildMember: Store roles as JSON
            builder.Entity<DiscordGuildMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Roles)
                    .HasConversion(rolesConverter)
                    .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));
                entity.Property(e => e.Nick).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.DiscordUserId).HasMaxLength(50).IsRequired();
            });

            // GameLogEvent
            builder.Entity<GameLogEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.EventType).HasMaxLength(100);
                entity.Property(e => e.Payload).HasMaxLength(2000).IsRequired(false);
                entity.Property(e => e.RawLine).HasMaxLength(2000).IsRequired();
            });
            
            // ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                // Don't override the primary key - Identity uses Id, not UserId
                entity.HasIndex(e => e.DiscordId).IsUnique();
                entity.Property(e => e.DiscordId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscordName).HasMaxLength(100);
                entity.Property(e => e.DiscordDiscriminator).HasMaxLength(10);
                entity.Property(e => e.DiscordAvatar).HasMaxLength(200);
                entity.Property(e => e.DiscordAccessToken).HasMaxLength(500);
                entity.Property(e => e.DiscordRefreshToken).HasMaxLength(500);
                entity.Property(e => e.DiscordRoles).HasMaxLength(2000).IsRequired(false);
                entity.Property(e => e.DiscordTokenExpiresAt).IsRequired(false);
                entity.Property(e => e.StarCitizenCharacterName).HasMaxLength(100);
                entity.Property(e => e.GameLogFilePath).HasMaxLength(500);
                entity.Property(e => e.StarCitizenApiKey).HasMaxLength(500);
                entity.Property(e => e.StarCitizenApiUrl).HasMaxLength(500);
                entity.Property(e => e.StarCitizenOrgRank).HasMaxLength(100);
                entity.Property(e => e.TradingApiKey).HasMaxLength(500);
                entity.Property(e => e.TradingApiUrl).HasMaxLength(500);
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.AvatarUrl).HasMaxLength(200);
                entity.Property(e => e.Bio).HasMaxLength(500);
                entity.Property(e => e.Theme).HasMaxLength(50);
                entity.Property(e => e.Language).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.LastLoginAt).IsRequired(false);
                entity.Property(e => e.LastLogoutAt).IsRequired(false);
            });

            // Kill entity
            builder.Entity<Kill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KillerId).HasMaxLength(50);
                entity.Property(e => e.KillerName).HasMaxLength(100);
                entity.Property(e => e.VictimId).HasMaxLength(50);
                entity.Property(e => e.VictimName).HasMaxLength(100);
                entity.Property(e => e.Weapon).HasMaxLength(100);
                entity.Property(e => e.GameLogSource).HasMaxLength(100);
                entity.Property(e => e.UserId).HasMaxLength(450); // Identity uses 450 for user IDs
                entity.Property(e => e.KillType).IsRequired();
                entity.Property(e => e.IsPvp).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200);
                
                // Foreign key relationship to ApplicationUser
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull); // Use SetNull instead of Cascade
            });
        }
    }
}