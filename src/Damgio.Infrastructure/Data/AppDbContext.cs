using Damgio.Core.Entities;
using Damgio.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Damgio.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<LoyaltyProgram> Programs => Set<LoyaltyProgram>();
    public DbSet<StampCard> StampCards => Set<StampCard>();
    public DbSet<StampEvent> StampEvents => Set<StampEvent>();
    public DbSet<ShopMember> ShopMembers => Set<ShopMember>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        });

        builder.Entity<Shop>(e =>
        {
            e.ToTable("Shops");
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.Property(s => s.Slug).HasMaxLength(50).IsRequired();
            e.Property(s => s.Address).HasMaxLength(250);
            e.HasIndex(s => s.Slug).IsUnique();
        });

        builder.Entity<LoyaltyProgram>(e =>
        {
            e.ToTable("Programs", t =>
            {
                t.HasCheckConstraint("CK_Programs_RequiredStamps", "\"RequiredStamps\" BETWEEN 1 AND 50");
                t.HasCheckConstraint("CK_Programs_Cooldown", "\"StampCooldownSeconds\" >= 0");
            });
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.RewardName).HasMaxLength(100).IsRequired();

            // Shop 1─N Programs
            e.HasOne(p => p.Shop)
                .WithMany(s => s.Programs)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StampCard>(e =>
        {
            e.ToTable("StampCards", t =>
                t.HasCheckConstraint("CK_StampCards_NonNegative",
                    "\"CurrentStamps\" >= 0 AND \"LifetimeStamps\" >= 0"));

            // Program 1─N StampCards
            e.HasOne(c => c.Program)
                .WithMany(p => p.StampCards)
                .HasForeignKey(c => c.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser 1─N StampCards (no navigation in Core to keep it Identity-free)
            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One card per customer per program
            e.HasIndex(c => new { c.UserId, c.ProgramId }).IsUnique();

            // Optimistic concurrency via PostgreSQL xmin system column
            e.Property<uint>("Version").IsRowVersion();
        });

        builder.Entity<StampEvent>(e =>
        {
            e.ToTable("StampEvents");
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(20);

            // StampCard 1─N StampEvents
            e.HasOne(x => x.StampCard)
                .WithMany(c => c.Events)
                .HasForeignKey(x => x.StampCardId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Device)
                .WithMany()
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.ActorUserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(x => new { x.StampCardId, x.CreatedAt });
        });

        builder.Entity<ShopMember>(e =>
        {
            e.ToTable("ShopMembers");
            e.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);

            // Shop 1─N ShopMembers
            e.HasOne(m => m.Shop)
                .WithMany(s => s.Members)
                .HasForeignKey(m => m.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser 1─N ShopMembers
            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(m => new { m.ShopId, m.UserId }).IsUnique();
        });

        builder.Entity<Device>(e =>
        {
            e.ToTable("Devices");
            e.Property(d => d.Name).HasMaxLength(50).IsRequired();
            e.Property(d => d.KeyHash).HasMaxLength(64).IsFixedLength().IsRequired();
            e.HasIndex(d => d.KeyHash).IsUnique();

            // Shop 1─N Devices
            e.HasOne(d => d.Shop)
                .WithMany(s => s.Devices)
                .HasForeignKey(d => d.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.Program)
                .WithMany()
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampTimestamps();
        return base.SaveChanges();
    }

    /// <summary>Fills CreatedAt / UpdatedAt in UTC so callers never forget them.</summary>
    private void StampTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty("CreatedAt") is not null)
            {
                var created = entry.Property("CreatedAt");
                if (created.CurrentValue is DateTime dt && dt == default)
                    created.CurrentValue = now;
            }

            if (entry.State is EntityState.Added or EntityState.Modified
                && entry.Metadata.FindProperty("UpdatedAt") is not null)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
    }
}
