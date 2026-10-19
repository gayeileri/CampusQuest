namespace CampusQuest.Mobile;

public partial class RegisterPage : ContentPage
{
    private readonly Services.ApiService _apiService;

    public RegisterPage()
    {
        InitializeComponent();
        _apiService = new Services.ApiService();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;
        var confirm = PasswordConfirmEntry.Text;
        var termsAccepted = TermsCheckBox.IsChecked;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanları doldurun.", "Tamam");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("Hata", "Şifreler eşleşmiyor.", "Tamam");
            return;
        }

        if (!termsAccepted)
        {
            await DisplayAlert("Hata", "Lütfen kullanım koşullarını kabul edin.", "Tamam");
            return;
        }

        RegisterButton.IsEnabled = false;
        RegisterButton.Text = "Oluşturuluyor...";

        var response = await _apiService.RegisterAsync(name, email, password);

        if (response != null)
        {
            Preferences.Set("AuthToken", response.Token);
            if (Application.Current != null)
                Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Kayıt Başarısız", "E-posta kullanımda olabilir veya bilgiler eksik.", "Tamam");
            RegisterButton.IsEnabled = true;
            RegisterButton.Text = "Hesap Oluştur ➔";
        }
    }

    private void OnLoginTapped(object sender, EventArgs e)
    {
        // Swap main page to login
        if (Application.Current != null)
            Application.Current.MainPage = new LoginPage();
    }

    private void OnTogglePasswordVisibilityTapped(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        PasswordEyeIcon.Text = PasswordEntry.IsPassword ? "👁️" : "🙈";
    }

    private void OnToggleConfirmPasswordVisibilityTapped(object sender, EventArgs e)
    {
        PasswordConfirmEntry.IsPassword = !PasswordConfirmEntry.IsPassword;
        PasswordConfirmEyeIcon.Text = PasswordConfirmEntry.IsPassword ? "👁️" : "🙈";
    }
}
