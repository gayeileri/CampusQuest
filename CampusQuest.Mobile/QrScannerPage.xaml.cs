using ZXing.Net.Maui;
using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile;

public partial class QrScannerPage : ContentPage
{
    private readonly int _missionId;
    private readonly string _missionTitle;
    private readonly double _targetLat;
    private readonly double _targetLng;
    private readonly ApiService _apiService;

    public QrScannerPage(int missionId, string missionTitle, double targetLat, double targetLng)
    {
        InitializeComponent();
        _missionId = missionId;
        _missionTitle = missionTitle;
        _targetLat = targetLat;
        _targetLng = targetLng;
        _apiService = new ApiService();

        BarcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false,
            TryHarder = true,
            TryInverted = true,
            DelayBetweenAnalyzingFrames = 150
        };
    }

    private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var firstCode = e.Results.FirstOrDefault();
        if (firstCode == null) return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            BarcodeReader.IsDetecting = false;
            string scannedData = firstCode.Value;

            // 1. KONTROL: QR Kod Geçerli mi?
            if (!scannedData.Contains("CAMPUSQUEST"))
            {
                await DisplayAlert("Geçersiz QR!", "Bu QR kod bir CampusQuest görev noktasına ait değil.", "Tekrar Dene");
                BarcodeReader.IsDetecting = true;
                return;
            }

            // 2. KONTROL: Kullanıcı Görev Noktasına Yakın mı? (Maks 100 Metre)
            var userLocation = await GetValidUserLocationAsync();
            if (userLocation == null)
            {
                BarcodeReader.IsDetecting = true;
                return;
            }

            // 3. KONTROL: Backend API İsteği (Veritabanında XP Kaydı)
            bool isApiSuccess = await _apiService.CompleteMissionAsync(userId: 1, missionId: _missionId, qrData: scannedData, latitude: userLocation.Latitude, longitude: userLocation.Longitude);

            if (isApiSuccess)
            {
                await DisplayAlert("🎉 TEBRİKLER!", $"'{_missionTitle}' görevini başardınız!\n\nXP hesabınıza tanımlandı.", "Harika!");
            }
            else
            {
                await DisplayAlert("Bilgi", $"'{_missionTitle}' konumu ve QR kodu doğrulandı fakat sunucuyla iletişim kurulamadı.", "Tamam");
            }

            await Navigation.PopAsync();
        });
    }

    private async Task<Location?> GetValidUserLocationAsync()
    {
        try
        {
            var userLocation = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (userLocation == null)
            {
                await DisplayAlert("Konum Alınamadı", "GPS konumunuz belirlenemedi.", "Tamam");
                return null;
            }

            var targetLocation = new Location(_targetLat, _targetLng);

            // MAUI DistanceUnits.Kilometers hesabı yapıp metreye çeviriyoruz
            double distanceInKilometers = Location.CalculateDistance(userLocation, targetLocation, DistanceUnits.Kilometers);
            double distanceInMeters = distanceInKilometers * 1000.0;

            // 100 metreden uzaktaysa uyarı ver
            if (distanceInMeters > 100)
            {
                await DisplayAlert("Uzaklığınız Çok Fazla!",
                    $"Görev noktasına {Math.Round(distanceInMeters)} metre uzaktasınız. Görevi yapmak için alana yaklaşmalısınız.",
                    "Tamam");
                return null;
            }

            return userLocation;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Konum doğrulanırken hata oluştu: {ex.Message}", "Tamam");
            return null;
        }
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private bool _isFlashOn = false;
    private void FlashButton_Clicked(object sender, EventArgs e)
    {
        _isFlashOn = !_isFlashOn;
        BarcodeReader.IsTorchOn = _isFlashOn;
        FlashButton.Text = _isFlashOn ? "🔦 Fener (Açık)" : "🔦 Fener";
        FlashButton.BackgroundColor = _isFlashOn ? Color.FromArgb("#88FFD600") : Color.FromArgb("#44FFFFFF");
    }
}