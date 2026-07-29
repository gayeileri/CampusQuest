using Microsoft.AspNetCore.Mvc;

namespace CampusQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    // POST: api/missions/complete
    [HttpPost("complete")]
    public IActionResult CompleteMission([FromBody] CompleteMissionRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { Success = false, Message = "Geçersiz istek verisi." });
        }

        // TODO: Gerçek doğrulama; burada basitçe gelen veriyi kabul edip test yanıtı dönüyoruz.
        return Ok(new
        {
            Success = true,
            Message = "Tebrikler! Görev başarıyla tamamlandı.",
            UserId = request.UserId,
            MissionId = request.MissionId,
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
    public string QrData { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}