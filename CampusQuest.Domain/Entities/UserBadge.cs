namespace CampusQuest.Domain.Entities;

public class UserBadge
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign keys
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BadgeId { get; set; }
    public Badge? Badge { get; set; }

    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
}
