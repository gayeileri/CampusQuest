using System.Security.Claims;
using CampusQuest.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusQuest.API.Controllers;

[ApiController, Authorize, Route("api/profile")]
public sealed class ProfileController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(claim, out var userId)) return Unauthorized();
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) return NotFound();
        var missions = await db.UserMissions.AsNoTracking().Where(x => x.UserId == userId)
            .Include(x => x.Mission).OrderByDescending(x => x.CompletedAt)
            .Select(x => new { x.MissionId, title = x.Mission!.Title, xp = x.Mission.XpReward, x.CompletedAt })
            .ToListAsync(cancellationToken);
        var badges = new List<object>();
        if (missions.Count >= 1) badges.Add(new { name = "İlk Adım", description = "İlk görevi tamamladın.", icon = "🏅" });
        if (missions.Count >= 3) badges.Add(new { name = "Kampüs Kaşifi", description = "Üç görevi tamamladın.", icon = "🧭" });
        if (user.TotalXp >= 500) badges.Add(new { name = "XP Ustası", description = "500 XP kazandın.", icon = "⭐" });
        return Ok(new { user.Username, user.Email, totalXp = user.TotalXp, user.Level, avatarUrl = (string?)null, completedMissions = missions, badges });
    }
}
