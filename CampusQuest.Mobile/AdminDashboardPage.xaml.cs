namespace CampusQuest.Mobile;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
            Application.Current.MainPage = new LoginPage();
    }
}
