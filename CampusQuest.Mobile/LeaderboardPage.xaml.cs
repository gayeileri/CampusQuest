using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile;

public partial class LeaderboardPage : ContentPage
{
    private readonly ApiService _apiService;

    public LeaderboardPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var users = await _apiService.GetLeaderboardAsync();
        
        // Eğer API'den veri gelemezse (örn: sunucu kapalıysa), tasarımın görünmesi için dummy data ekleyelim.
        if (users == null || users.Count == 0)
        {
            users = new List<LeaderboardUser>
            {
                new LeaderboardUser { Rank = 1, Username = "Zeynep", TotalXp = 3120, Level = 12 },
                new LeaderboardUser { Rank = 2, Username = "Burak", TotalXp = 2450, Level = 10 },
                new LeaderboardUser { Rank = 3, Username = "Ali", TotalXp = 1980, Level = 8 },
                new LeaderboardUser { Rank = 4, Username = "Ayşe Y.", TotalXp = 1750, Level = 7 },
                new LeaderboardUser { Rank = 5, Username = "Mehmet K.", TotalXp = 1620, Level = 6 },
                new LeaderboardUser { Rank = 42, Username = "Sen", TotalXp = 850, Level = 3 }
            };
        }

        var viewModel = new ViewModels.LeaderboardViewModel(users, "Sen");
        BindingContext = viewModel;
    }

    private void OnHaftalikTapped(object sender, EventArgs e)
    {
        TabHaftalik.BackgroundColor = Color.FromArgb("#1B5E20");
        TabHaftalikLabel.TextColor = Colors.White;
        TabTumZamanlar.BackgroundColor = Colors.Transparent;
        TabTumZamanlarLabel.TextColor = Color.FromArgb("#757575");
        // Haftalık veriler (şimdilik aynı veriyi gösteriyor)
    }

    private void OnTumZamanlarTapped(object sender, EventArgs e)
    {
        TabTumZamanlar.BackgroundColor = Color.FromArgb("#1B5E20");
        TabTumZamanlarLabel.TextColor = Colors.White;
        TabHaftalik.BackgroundColor = Colors.Transparent;
        TabHaftalikLabel.TextColor = Color.FromArgb("#757575");
        // Tüm zamanlar veriler (şimdilik aynı veriyi gösteriyor)
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }
}
