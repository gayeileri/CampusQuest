namespace CampusQuest.Domain.Entities;

public class Mission
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int XpReward { get; set; }

    // Location Data (Coordinates for campus spots)
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; } = 20.0; // Allowed range to complete the mission

    // QR Kod Doğrulama Kodu
    public string QrCodeSecret { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Navigation for many-to-many via UserMission
    public ICollection<UserMission>? UserMissions { get; set; }
}