namespace CampusQuest.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    // Gamification Properties
    public int TotalXp { get; set; } = 0;
    // Backwards-compatible alias used by services
    public int Xp { get => TotalXp; set => TotalXp = value; }
    public int Level { get; set; } = 1;

    // Audit Properties
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties for relations
    public ICollection<UserMission>? CompletedMissions { get; set; }
    public ICollection<UserBadge>? EarnedBadges { get; set; }
}