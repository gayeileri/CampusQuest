using System.Globalization;
using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile;

public partial class TasksPage : ContentPage
{
    private readonly Services.ApiService _apiService;
    private List<TaskMapItem>? _allTasks;
    private CancellationTokenSource? _locationCts;

    public TasksPage()
    {
        InitializeComponent();
        _apiService = new Services.ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Görevleri API'den çek
        _allTasks = await _apiService.GetTasksAsync();
        
        // Eğer API'den veri gelmezse, ekranda görünmesi için dummy görevler ekle
        if (_allTasks == null || _allTasks.Count == 0)
        {
            _allTasks = new List<TaskMapItem>
            {
                new TaskMapItem { Id = Guid.NewGuid(), Title = "Kütüphane Ziyareti", Description = "Merkez kütüphaneye git ve oradaki QR kodu okut.", XpReward = 50, Latitude = 41.0, Longitude = 29.0 },
                new TaskMapItem { Id = Guid.NewGuid(), Title = "Spor Salonu Keşfi", Description = "Kampüs spor salonunu bul ve check-in yap.", XpReward = 75, Latitude = 41.001, Longitude = 29.002 },
                new TaskMapItem { Id = Guid.NewGuid(), Title = "Kafeterya Buluşması", Description = "Öğle arasında ana kafeteryaya uğra.", XpReward = 30, Latitude = 41.002, Longitude = 29.001 }
            };
        }
        
        PopulateAktifList();

        // Tamamlanan görevleri profildeki CompletedMissions'dan çek
        var profile = await _apiService.GetProfileAsync();
        var completedList = profile?.CompletedMissions;

        // Eğer tamamlanan görev gelmezse tasarım görünsün diye dummy ekle
        if (completedList == null || completedList.Count == 0)
        {
            completedList = new List<CompletedMission>
            {
                new CompletedMission { MissionId = Guid.NewGuid(), Title = "Öğrenci İşleri Ziyareti", Xp = 100, CompletedAt = DateTime.Now.AddDays(-2) },
                new CompletedMission { MissionId = Guid.NewGuid(), Title = "Kampüs Turu", Xp = 200, CompletedAt = DateTime.Now.AddDays(-5) }
            };
        }

        TamamlananEmpty.IsVisible = false;
        TamamlananList.Children.Clear();
        foreach (var m in completedList)
        {
            TamamlananList.Children.Add(CreateCompletedCard(m));
        }

        // Canlı konum takibini başlat
        StartLocationTracking();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopLocationTracking();
    }

    // ===================== TAB GEÇİŞLERİ =====================

    private void OnTabAktifTapped(object sender, EventArgs e)
    {
        SetActiveTab("aktif");
    }

    private void OnTabGunlukTapped(object sender, EventArgs e)
    {
        SetActiveTab("gunluk");
    }

    private void OnTabTamamlananTapped(object sender, EventArgs e)
    {
        SetActiveTab("tamamlanan");
    }

    private void SetActiveTab(string tab)
    {
        // Renkleri ayarla
        TabAktif.BackgroundColor = tab == "aktif" ? Color.FromArgb("#1B5E20") : Color.FromArgb("#E8F5E9");
        TabGunluk.BackgroundColor = tab == "gunluk" ? Color.FromArgb("#1B5E20") : Color.FromArgb("#E8F5E9");
        TabTamamlanan.BackgroundColor = tab == "tamamlanan" ? Color.FromArgb("#1B5E20") : Color.FromArgb("#E8F5E9");

        // Label renkleri (Tab'ın ilk child'ını al)
        SetTabLabelColor(TabAktif, tab == "aktif");
        SetTabLabelColor(TabGunluk, tab == "gunluk");
        SetTabLabelColor(TabTamamlanan, tab == "tamamlanan");

        // Görünürlük
        AktifContent.IsVisible = tab == "aktif";
        GunlukContent.IsVisible = tab == "gunluk";
        TamamlananContent.IsVisible = tab == "tamamlanan";
    }

    private void SetTabLabelColor(Border border, bool isActive)
    {
        if (border.Content is Label label)
        {
            label.TextColor = isActive ? Colors.White : Color.FromArgb("#1B5E20");
        }
    }

    // ===================== AKTİF GÖREV LİSTESİ =====================

    private void PopulateAktifList()
    {
        AktifList.Children.Clear();

        if (_allTasks == null || _allTasks.Count == 0)
        {
            AktifList.Children.Add(new Label
            {
                Text = "Şu an aktif görev bulunamadı.",
                FontSize = 14,
                TextColor = Color.FromArgb("#9E9E9E"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 40, 0, 0)
            });
            return;
        }

        foreach (var task in _allTasks)
        {
            AktifList.Children.Add(CreateTaskCard(task));
        }
    }

    private View CreateTaskCard(TaskMapItem task)
    {
        var card = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.White,
            Stroke = Colors.Transparent,
            Padding = 20
        };

        var stack = new VerticalStackLayout { Spacing = 12 };

        // Başlık + XP
        var titleGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        titleGrid.Add(new Label { Text = task.Title, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") }, 0);
        titleGrid.Add(new Label { Text = $"+{task.XpReward} XP", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#4CAF50") }, 1);
        stack.Children.Add(titleGrid);

        // Açıklama
        stack.Children.Add(new Label { Text = task.Description, FontSize = 14, TextColor = Color.FromArgb("#757575") });

        // Konum bilgisi
        stack.Children.Add(new Label { Text = $"📍 {task.Latitude.ToString("F4", CultureInfo.InvariantCulture)}, {task.Longitude.ToString("F4", CultureInfo.InvariantCulture)}", FontSize = 11, TextColor = Color.FromArgb("#9E9E9E") });

        // Haritada Gör Butonu
        var btn = new Button
        {
            Text = "🗺️ Haritada Gör",
            BackgroundColor = Color.FromArgb("#1B5E20"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 8,
            HeightRequest = 44,
            Margin = new Thickness(0, 8, 0, 0)
        };
        btn.Clicked += async (s, e) =>
        {
            // Keşfet (Harita) sekmesine git
            await Shell.Current.GoToAsync("//MainPage");
        };
        stack.Children.Add(btn);

        card.Content = stack;
        return card;
    }

    // ===================== TAMAMLANAN GÖREV KARTI =====================

    private View CreateCompletedCard(CompletedMission mission)
    {
        var card = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.White,
            Stroke = Colors.Transparent,
            Padding = 20
        };

        var stack = new VerticalStackLayout { Spacing = 8 };

        var titleGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        titleGrid.Add(new Label { Text = $"✅ {mission.Title}", FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") }, 0);
        titleGrid.Add(new Label { Text = $"+{mission.Xp} XP", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#4CAF50") }, 1);
        stack.Children.Add(titleGrid);

        stack.Children.Add(new Label { Text = $"Tamamlandı: {mission.CompletedAt:dd MMM yyyy HH:mm}", FontSize = 12, TextColor = Color.FromArgb("#9E9E9E") });

        card.Content = stack;
        return card;
    }

    // ===================== CANLI KONUM TAKİBİ =====================

    private void StartLocationTracking()
    {
        _locationCts = new CancellationTokenSource();
        var token = _locationCts.Token;

        // Arka planda periyodik konum güncelle
        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            LocationStatusLabel.Text = "⚠️ Konum izni verilmedi";
                        });
                        break;
                    }

                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5));
                    var location = await Geolocation.Default.GetLocationAsync(request, token);

                    if (location != null)
                    {
                        var lat = location.Latitude.ToString("F6", CultureInfo.InvariantCulture);
                        var lng = location.Longitude.ToString("F6", CultureInfo.InvariantCulture);

                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            LocationStatusLabel.Text = $"📍 Konum aktif: {lat}, {lng} (güncellendi: {DateTime.Now:HH:mm:ss})";
                        });
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Konum hatası: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        LocationStatusLabel.Text = $"⚠️ Konum alınamadı: {ex.Message}";
                    });
                }

                // 5 saniyede bir güncelle
                try { await Task.Delay(5000, token); } catch { break; }
            }
        }, token);
    }

    private void StopLocationTracking()
    {
        _locationCts?.Cancel();
        _locationCts?.Dispose();
        _locationCts = null;
    }

}
