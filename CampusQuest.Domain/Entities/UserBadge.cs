namespace CampusQuest.Domain.Entities;

public class UserBadge
{
    public Guid Id { get; set; }

    // Foreign keys
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BadgeId { get; set; }
    public Badge? Badge { get; set; }

    public DateTime EarnedAt { get; set; } = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
