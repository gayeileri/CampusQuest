using System.Globalization;

namespace CampusQuest.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        SetupWebViewEvents();
        LoadMap();
    }

    private void SetupWebViewEvents()
    {
        // QR Kodu Tara butonuna basıldığında tetiklenen yönlendirme dinleyicisi
        MapWebView.Navigating += async (sender, e) =>
        {
            if (e.Url.StartsWith("app://scanqr"))
            {
                e.Cancel = true; // WebView içi yönlendirmeyi durdur

                var uri = new Uri(e.Url);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string title = query["title"] ?? "Görev";

                // URL'den pinin koordinatlarını okuyoruz
                double.TryParse(query["lat"], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat);
                double.TryParse(query["lng"], NumberStyles.Any, CultureInfo.InvariantCulture, out double lng);

                // Kamera izni iste
                var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    await DisplayAlert("İzin Gerekli", "QR kod okuyabilmek için kamera izni vermelisiniz.", "Tamam");
                    return;
                }

                // Konum izni iste (QR doğrulama için kullanıcı konumu gerekecek)
                var locStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (locStatus != PermissionStatus.Granted)
                {
                    await DisplayAlert("İzin Gerekli", "Görevi kontrol edebilmek için konum izni vermelisiniz.", "Tamam");
                    return;
                }

                // Koordinatları ve başlığı QR tarama sayfasına gönderiyoruz
                await Navigation.PushAsync(new QrScannerPage(title, lat, lng));
            }
        };

        // Harita tamamen yüklendikten sonra canlı GPS konumunu getir
        MapWebView.Loaded += async (sender, e) =>
        {
            await Task.Delay(2000);
            await GetUserLocationAndShowOnMapAsync();
        };
    }

    private async Task GetUserLocationAndShowOnMapAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) return;

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                string latStr = location.Latitude.ToString(CultureInfo.InvariantCulture);
                string lngStr = location.Longitude.ToString(CultureInfo.InvariantCulture);

                string jsCall = $"setUserLocation({latStr}, {lngStr});";
                await MapWebView.EvaluateJavaScriptAsync(jsCall);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Konum hatası: {ex.Message}");
        }
    }

    private void LoadMap()
    {
        var htmlSource = new HtmlWebViewSource();
        htmlSource.Html = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8' />
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
            <style>
                html, body, #map { height: 100%; margin: 0; padding: 0; background-color: #f0f0f0; }
                .popup-card { text-align: center; font-family: sans-serif; padding: 5px; }
                .popup-title { font-size: 14px; font-weight: bold; color: #1b5e20; margin-bottom: 4px; }
                .popup-desc { font-size: 12px; color: #555; margin-bottom: 6px; }
                .popup-xp { font-weight: bold; color: #d32f2f; font-size: 13px; margin-bottom: 8px; }
                .popup-btn {
                    background-color: #2e7d32;
                    color: white;
                    border: none;
                    padding: 8px 14px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-weight: bold;
                    width: 100%;
                }
                .popup-btn:active { background-color: #1b5e20; }
            </style>
        </head>
        <body>
            <div id='map'></div>

            <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
            <script>
                var map = L.map('map').setView([37.0592, 35.3552], 15);

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    maxZoom: 19,
                    attribution: '© OpenStreetMap'
                }).addTo(map);

                var missions = [
                    { id: 1, title: 'Merkezi Kütüphane', description: 'Kütüphanedeki gizli QR kodu bul!', xp: 150, lat: 37.0592, lng: 35.3552 },
                    { id: 2, title: 'Merkez Yemekhane', description: 'Yemekhanedeki QR kodu tara!', xp: 100, lat: 37.0565, lng: 35.3530 },
                    { id: 3, title: 'Mithat Özsan Amfisi', description: 'Amfi girişindeki QR kodu yakala!', xp: 120, lat: 37.0610, lng: 35.3570 },
                    { id: 4, title: 'Mediko Sağlık Merkezi', description: 'Danışmadaki QR kodu okut!', xp: 80, lat: 37.0580, lng: 35.3510 }
                ];

                missions.forEach(function(m) {
                    var popupHtml = `
                        <div class='popup-card'>
                            <div class='popup-title'>${m.title}</div>
                            <div class='popup-desc'>${m.description}</div>
                            <div class='popup-xp'>Ödül: +${m.xp} XP</div>
                            <button class='popup-btn' onclick='openQRScanner(${m.id}, ""${m.title}"", ${m.lat}, ${m.lng})'>📷 QR Kodu Tara</button>
                        </div>
                    `;

                    L.marker([m.lat, m.lng]).addTo(map).bindPopup(popupHtml);
                });

                function openQRScanner(id, title, lat, lng) {
                    window.location.href = 'app://scanqr?id=' + id + '&title=' + encodeURIComponent(title) + '&lat=' + lat + '&lng=' + lng;
                }

                var userMarker;
                function setUserLocation(lat, lng) {
                    if (userMarker) {
                        map.removeLayer(userMarker);
                    }
                    userMarker = L.circleMarker([lat, lng], {
                        radius: 9,
                        fillColor: '#1E88E5',
                        color: '#FFFFFF',
                        weight: 3,
                        opacity: 1,
                        fillOpacity: 0.9
                    }).addTo(map).bindPopup('<b>Buradasınız</b>');

                    map.panTo([lat, lng]);
                }
            </script>
        </body>
        </html>";

        MapWebView.Source = htmlSource;
    }
}