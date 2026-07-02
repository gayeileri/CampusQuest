using Microsoft.EntityFrameworkCore;

namespace CampusQuest.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CampusQuest.Domain.Entities.User> Users { get; set; }
    public DbSet<CampusQuest.Domain.Entities.Mission> Missions { get; set; }
    public DbSet<CampusQuest.Domain.Entities.Badge> Badges { get; set; }
    public DbSet<CampusQuest.Domain.Entities.UserMission> UserMissions { get; set; }
    public DbSet<CampusQuest.Domain.Entities.UserBadge> UserBadges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Many-to-Many Relationship for User - Mission
        modelBuilder.Entity<CampusQuest.Domain.Entities.UserMission>()
            .HasOne(um => um.User)
            .WithMany(u => u.CompletedMissions)
            .HasForeignKey(um => um.UserId);

        modelBuilder.Entity<CampusQuest.Domain.Entities.UserMission>()
            .HasOne(um => um.Mission)
            .WithMany()
            .HasForeignKey(um => um.MissionId);

        // Configure Many-to-Many Relationship for User - Badge
        modelBuilder.Entity<CampusQuest.Domain.Entities.UserBadge>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.EarnedBadges)
            .HasForeignKey(ub => ub.UserId);

        modelBuilder.Entity<CampusQuest.Domain.Entities.UserBadge>()
            .HasOne(ub => ub.Badge)
            .WithMany()
            .HasForeignKey(ub => ub.BadgeId);
    }
}