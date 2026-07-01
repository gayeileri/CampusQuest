namespace CampusQuest.Domain.Entities;

public class Badge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty; // URL for mobile app to render the badge icon

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}