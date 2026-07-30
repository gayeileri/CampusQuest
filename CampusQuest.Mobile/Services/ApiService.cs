using System.Net.Http.Json;

namespace CampusQuest.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    // launchSettings.json dosyaındaki 5105 portuna göre güncellendi:
    private const string BaseUrl = "http://10.0.2.2:5105/api/";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// QR okutulup konum doğrulandığında backend'e görevin tamamlandığını bildirir.
    /// </summary>
    public async Task<bool> CompleteMissionAsync(int userId, int missionId, string qrData, double latitude, double longitude)
    {
        try
        {
            var requestData = new
            {
                UserId = userId,
                MissionId = missionId,
                QrData = qrData,
                Latitude = latitude,
                Longitude = longitude
            };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}missions/complete", requestData);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API İstek Hatası: {ex.Message}");
            return false;
        }
    }
}