using CampusQuest.Mobile.ViewModels;

namespace CampusQuest.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel; // ViewModel is provided by DI
        }
    }
}
