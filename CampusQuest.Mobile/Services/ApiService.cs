using System.Net.Http.Json;

namespace CampusQuest.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    
    // Gerçek cihazla test edildiği için bilgisayarın yerel ağ IP adresi (192.168.1.186) eklendi.
    private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android 
        ? "http://192.168.1.186:5105/api/" 
        : "http://localhost:5105/api/";
    
    // Auth token'ı saklamak için (Login yapıldığında buraya atanacak)
    public string? AuthToken { get; set; }
    
    // Son hata mesajını sakla (kullanıcıya gösterilebilir)
    public string? LastError { get; set; }

    public ApiService()
    {
        var handler = new HttpClientHandler
        {
            // SSL sertifika doğrulamasını geliştirme ortamında atla
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    private void SetAuthorizationHeader()
    {
        // Önce property'de var mı bak, yoksa Preferences'tan çek
        if (string.IsNullOrEmpty(AuthToken))
        {
            AuthToken = Preferences.Get("AuthToken", string.Empty);
        }

        if (!string.IsNullOrEmpty(AuthToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);
        }
    }

    public async Task<bool> CompleteMissionAsync(int userId, int missionId, string qrData, double latitude, double longitude)
    {
        try
        {
            SetAuthorizationHeader();
            var requestData = new { UserId = userId, MissionId = missionId, QrData = qrData, Latitude = latitude, Longitude = longitude };
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}missions/complete", requestData);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Hata: {ex.Message}");
            return false;
        }
    }

    // --- AUTH (GİRİŞ/KAYIT) ENDPOINTLERİ ---

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new { Email = email, Password = password };
            System.Diagnostics.Debug.WriteLine($"Login isteği gönderiliyor: {BaseUrl}auth/login");
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}auth/login", request);
            System.Diagnostics.Debug.WriteLine($"Login yanıt kodu: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    AuthToken = result.Token;
                    return result;
                }
            }
            
            // Başarısız yanıtın gövdesini oku
            var errorBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Login hata yanıtı: {errorBody}");
            LastError = $"Sunucu yanıtı: {(int)response.StatusCode} - {errorBody}";
            return null;
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"Login Ağ Hatası: {httpEx.Message}");
            LastError = $"Sunucuya bağlanılamadı. API çalışıyor mu? ({httpEx.Message})";
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login Hatası: {ex.Message}");
            LastError = $"Beklenmeyen hata: {ex.Message}";
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var request = new { Username = username, Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    AuthToken = result.Token;
                    return result;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Register Hatası: {ex.Message}");
            return null;
        }
    }

    // --- DİĞER EKLENEN ENDPOINTLER ---

    public async Task<ProfileResponse?> GetProfileAsync()
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ProfileResponse>($"{BaseUrl}profile");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Profile API Hata: {ex.Message}");
            return null;
        }
    }

    public async Task<List<LeaderboardUser>?> GetLeaderboardAsync()
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<LeaderboardUser>>($"{BaseUrl}leaderboard");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Leaderboard API Hata: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TaskMapItem>?> GetTasksAsync()
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<TaskMapItem>>($"{BaseUrl}tasks");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Tasks API Hata: {ex.Message}");
            return null;
        }
    }
}

// --- DTO MODELLERİ ---

public class ProfileResponse
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int Level { get; set; }
    public string? AvatarUrl { get; set; }
    public List<CompletedMission> CompletedMissions { get; set; } = new();
    public List<BadgeItem> Badges { get; set; } = new();
}

public class CompletedMission
{
    public Guid MissionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Xp { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class BadgeItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class LeaderboardUser
{
    public int Rank { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int Level { get; set; }
}

public class TaskMapItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int XpReward { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}