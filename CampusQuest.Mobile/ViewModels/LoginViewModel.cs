using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CampusQuest.Mobile.Services;

namespace CampusQuest.Mobile.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await ExecuteLoginAsync(), () => !IsBusy);
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public ICommand LoginCommand { get; }

        private async Task ExecuteLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please fill in all fields.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = await _authService.LoginAsync(Email, Password);

            IsBusy = false;

            if (result)
            {
                // Navigate to Main Application Page upon successful login
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "Invalid email or password.";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}