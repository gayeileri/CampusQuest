using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage()
    {
        InitializeComponent();
        _authService = new AuthService();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Error", "Fill all fields", "OK");
            return;
        }

        LoginButton.IsEnabled = false;
        bool isSuccess = await _authService.LoginAsync(email, password);
        LoginButton.IsEnabled = true;

        if (isSuccess)
            await DisplayAlertAsync("Success", "Welcome!", "OK");
        else
            await DisplayAlertAsync("Error", "Login failed", "OK");
    }

    private async void OnSignUpTapped(object? sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Info", "Sign up page soon...", "OK");
    }
}