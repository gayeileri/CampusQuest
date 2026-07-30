namespace CampusQuest.Domain.Entities;

public class Badge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty; // URL for mobile app to render the badge icon

    public DateTime CreatedAt { get; set; } = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Navigation for many-to-many via UserBadge
    public ICollection<UserBadge>? UserBadges { get; set; }
}
