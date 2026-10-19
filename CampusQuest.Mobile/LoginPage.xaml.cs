namespace CampusQuest.Mobile;

public partial class LoginPage : ContentPage
{
    private readonly Services.ApiService _apiService;

    public LoginPage()
    {
        InitializeComponent();
        _apiService = new Services.ApiService();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanları doldurun.", "Tamam");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Text = "Giriş Yapılıyor...";

        var response = await _apiService.LoginAsync(email, password);

        if (response != null)
        {
            Preferences.Set("AuthToken", response.Token);
            if (Application.Current != null)
                Application.Current.MainPage = new AppShell();
        }
        else
        {
            var errorMsg = _apiService.LastError ?? "E-posta veya şifre hatalı.";
            await DisplayAlert("Giriş Başarısız", errorMsg, "Tamam");
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Giriş Yap ➔";
        }
    }

    private void OnRegisterTapped(object sender, EventArgs e)
    {
        if (Application.Current != null)
            Application.Current.MainPage = new RegisterPage();
    }



    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Bilgi", "Lütfen önce e-posta adresinizi girin, ardından 'Şifremi Unuttum' butonuna tıklayın.", "Tamam");
        }
        else
        {
            await DisplayAlert("Başarılı", "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.", "Tamam");
        }
    }

    private void OnTogglePasswordVisibilityTapped(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        PasswordEyeIcon.Text = PasswordEntry.IsPassword ? "👁️" : "🙈";
    }
}
