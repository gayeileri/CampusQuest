namespace CampusQuest.Mobile;

public partial class ProfilePage : ContentPage
{
    private readonly Services.ApiService _apiService;

    public ProfilePage()
    {
        InitializeComponent();
        _apiService = new Services.ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var profile = await _apiService.GetProfileAsync();
        
        // Eğer API'den veri gelmezse (sunucu kapalıysa vs.), dummy bir profil gösterelim.
        if (profile == null)
        {
            profile = new Services.ProfileResponse
            {
                Username = "Gaye",
                Email = "gaye@campusquest.edu",
                TotalXp = 850,
                Level = 3,
                CompletedMissions = new List<Services.CompletedMission>(),
                Badges = new List<Services.BadgeItem>()
            };
        }

        // Eğer rozet yoksa dummy rozetler ekleyelim (tasarım boş görünmesin)
        if (profile.Badges == null || profile.Badges.Count == 0)
        {
            profile.Badges = new List<Services.BadgeItem>
            {
                new Services.BadgeItem { Icon = "🎒", Name = "Çaylak", Description = "Uygulamaya ilk giriş" },
                new Services.BadgeItem { Icon = "🗺️", Name = "Kaşif", Description = "5 farklı yer keşfedildi" },
                new Services.BadgeItem { Icon = "🔥", Name = "Ateşli", Description = "3 gün üst üste giriş" }
            };
        }

        BindingContext = profile;
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        var editProfilePage = new EditProfilePage();
        editProfilePage.BindingContext = this.BindingContext; // Pass current profile
        await Navigation.PushAsync(editProfilePage);
    }

    private async void OnNotificationsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NotificationSettingsPage());
    }

    private async void OnSecurityTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SecuritySettingsPage());
    }

    private async void OnTumunuGorTapped(object sender, EventArgs e)
    {
        var profile = BindingContext as Services.ProfileResponse;
        var badges = profile?.Badges ?? new List<Services.BadgeItem>();
        await Navigation.PushAsync(new BadgesPage(badges));
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Çıkış Yap", "Hesabınızdan çıkış yapmak istediğinize emin misiniz?", "Evet", "Hayır");
        if (answer)
        {
            Preferences.Remove("AuthToken");
            if (Application.Current != null)
                Application.Current.MainPage = new LoginPage();
        }
    }
}
