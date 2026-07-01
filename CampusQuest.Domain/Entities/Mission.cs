namespace CampusQuest.Domain.Entities;

public class Mission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int XpReward { get; set; }

    // Location Data (Coordinates for campus spots)
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; } = 20.0; // Allowed range to complete the mission

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}