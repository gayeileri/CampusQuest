namespace CampusQuest.Domain.Entities;

public class UserMission
{
    public Guid Id { get; set; }

    // Foreign keys
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid MissionId { get; set; }
    public Mission? Mission { get; set; }

    // Use a fixed UTC date to avoid dynamic defaults in the EF model
    public DateTime CompletedAt { get; set; } = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
