using System.Net.Http.Json;
using System.Text.Json;

namespace CampusQuest.Mobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // 🚨 NOTE: If you are using an Android Emulator, replace "localhost" with "10.0.2.2".
        // For Windows Machine or iOS Simulator, it uses our backend port 7168.
        private const string BaseUrl = "https://localhost:7168/api/auth/";

        public AuthService()
        {
            // Bypass SSL certificate validation errors for local development environment
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("register", new { username, email, password });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("login", new { email, password });
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // TODO: Store the authentication token or user session data locally in the future
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}