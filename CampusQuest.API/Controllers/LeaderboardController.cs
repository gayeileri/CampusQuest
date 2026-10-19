using CampusQuest.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusQuest.API.Controllers;

[ApiController, Authorize, Route("api/leaderboard")]
public sealed class LeaderboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var users = await db.Users.AsNoTracking()
        .OrderByDescending(x => x.TotalXp).ThenBy(x => x.Username)
        .Select(x => new { username = x.Username, totalXp = x.TotalXp, level = x.Level })
        .ToListAsync(cancellationToken);
        return Ok(users.Select((x, index) => new { rank = index + 1, x.username, x.totalXp, x.level }));
    }
}
