namespace CampusQuest.Mobile;

public partial class EditProfilePage : ContentPage
{
    public EditProfilePage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        SaveButton.IsEnabled = false;
        SaveButton.Text = "Kaydediliyor...";
        
        // Simüle edilmiş gecikme (gerçekte API'ye PUT isteği atılacak)
        await Task.Delay(1000);
        
        await DisplayAlert("Başarılı", "Profiliniz güncellendi.", "Tamam");
        await Navigation.PopAsync();
    }
}
