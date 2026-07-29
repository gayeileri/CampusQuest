using Microsoft.AspNetCore.Mvc;
using CampusQuest.Infrastructure.Persistence;
using CampusQuest.Domain.Entities;
using CampusQuest.Application.Services;

// For EF Core
using Microsoft.EntityFrameworkCore;

namespace CampusQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MissionsController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/missions/complete
    [HttpPost("complete")]
    public async Task<IActionResult> CompleteMission([FromBody] CompleteMissionRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { Success = false, Message = "Geçersiz istek verisi." });
        }

        // Find mission by QR secret match first
        Mission? mission = null;
        if (!string.IsNullOrEmpty(request.QrData))
        {
            mission = await _db.Missions.FirstOrDefaultAsync(m => m.QrCodeSecret == request.QrData);
        }

        // If not found by QR, try to find nearest mission within allowed radius
        if (mission == null)
        {
            var allMissions = await _db.Missions.ToListAsync();
            double minDist = double.MaxValue;
            foreach (var m in allMissions)
            {
                var dist = GeoService.DistanceInMeters(request.Latitude, request.Longitude, m.Latitude, m.Longitude);
                if (dist < minDist)
                {
                    minDist = dist;
                    mission = m;
                }
            }

            // if nearest mission is further than its allowed radius (or 100m cap), reject
            if (mission != null)
            {
                double allowed = Math.Max(mission.RadiusInMeters, 100.0);
                var actualDist = GeoService.DistanceInMeters(request.Latitude, request.Longitude, mission.Latitude, mission.Longitude);
                if (actualDist > allowed)
                {
                    return BadRequest(new { Success = false, Message = "Konumunuz göreve çok uzak. Görev konumuna yaklaşın." });
                }
            }
        }

        if (mission == null)
        {
            return BadRequest(new { Success = false, Message = "Görev bulunamadı." });
        }

        // Verify QR data matches mission secret
        if (!string.Equals(mission.QrCodeSecret, request.QrData, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { Success = false, Message = "QR doğrulanamadı." });
        }

        // Resolve or create a test user (mobile currently sends an int userId placeholder)
        User user = await _db.Users.FirstOrDefaultAsync() ?? new User { Username = "demo_user", Email = "demo@local" };
        if (user.Id == Guid.Empty)
        {
            user = new User { Username = "demo_user", Email = "demo@local" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        // Check if user already completed this mission
        var already = await _db.UserMissions.FirstOrDefaultAsync(um => um.UserId == user.Id && um.MissionId == mission.Id);
        if (already != null)
        {
            return Ok(new { Success = true, Message = "Zaten tamamlanmış görev.", UserId = user.Id, MissionId = mission.Id });
        }

        // Record completion
        var userMission = new UserMission { UserId = user.Id, MissionId = mission.Id, CompletedAt = DateTime.UtcNow };
        _db.UserMissions.Add(userMission);

        // Award XP
        user.TotalXp += mission.XpReward;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            Success = true,
            Message = "Tebrikler! Görev başarıyla tamamlandı.",
            UserId = user.Id,
            MissionId = mission.Id,
            QrData = request.QrData,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        });
    }
}

public class CompleteMissionRequest
{
    public int UserId { get; set; }
    public int MissionId { get; set; }
    public string? QrData { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}