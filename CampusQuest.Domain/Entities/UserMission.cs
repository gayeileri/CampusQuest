namespace CampusQuest.Domain.Entities;

public class UserMission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign keys
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid MissionId { get; set; }
    public Mission? Mission { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
