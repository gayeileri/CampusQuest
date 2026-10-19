using CampusQuest.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusQuest.Application.Services;
using System.Security.Claims;

namespace CampusQuest.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController(AppDbContext db) : ControllerBase
{
    // GET: api/tasks - coordinates are available only to authenticated users.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskMapItem>>> GetAsync(CancellationToken cancellationToken)
    {
        var tasks = await db.Missions
            .AsNoTracking()
            .OrderBy(mission => mission.Title)
            .Select(mission => new TaskMapItem(
                mission.Id,
                mission.Title,
                mission.Description,
                mission.XpReward,
                mission.Latitude,
                mission.Longitude))
            .ToListAsync(cancellationToken);

        return Ok(tasks);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteAsync(CompleteTaskRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var mission = await db.Missions.SingleOrDefaultAsync(x => x.Id == request.MissionId, cancellationToken);
        if (mission is null) return NotFound(new { message = "Görev bulunamadı." });
        if (!string.Equals(mission.QrCodeSecret, request.QrData, StringComparison.Ordinal))
            return BadRequest(new { message = "QR kod bu göreve ait değil." });

        var distance = GeoService.DistanceInMeters(request.Latitude, request.Longitude, mission.Latitude, mission.Longitude);
        if (distance > 50)
            return BadRequest(new { message = "Görev alanına yeterince yakın değilsiniz.", distanceMeters = Math.Round(distance) });

        if (await db.UserMissions.AnyAsync(x => x.UserId == userId && x.MissionId == mission.Id, cancellationToken))
            return Ok(new { message = "Bu görev zaten tamamlandı.", alreadyCompleted = true });

        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) return Unauthorized();
        db.UserMissions.Add(new CampusQuest.Domain.Entities.UserMission { UserId = userId, MissionId = mission.Id, CompletedAt = DateTime.UtcNow });
        user.TotalXp += mission.XpReward;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { message = "Görev tamamlandı.", earnedXp = mission.XpReward, totalXp = user.TotalXp });
    }
}

public sealed record TaskMapItem(
    Guid Id,
    string Title,
    string Description,
    int XpReward,
    double Latitude,
    double Longitude);

public sealed record CompleteTaskRequest(Guid MissionId, string QrData, double Latitude, double Longitude);
