namespace CampusQuest.Mobile;

public partial class AdminLoginPage : ContentPage
{
    public AdminLoginPage()
    {
        InitializeComponent();
    }

    private async void OnAdminLoginClicked(object sender, EventArgs e)
    {
        if (AdminUserEntry.Text == "admin" && AdminPassEntry.Text == "admin123")
        {
            if (Application.Current != null)
                Application.Current.MainPage = new NavigationPage(new AdminDashboardPage());
        }
        else
        {
            await DisplayAlert("Hata", "Yetkisiz giriş denemesi.", "Tamam");
        }
    }

    private void OnBackToStudentLoginTapped(object sender, EventArgs e)
    {
        if (Application.Current != null)
            Application.Current.MainPage = new LoginPage();
    }
}
