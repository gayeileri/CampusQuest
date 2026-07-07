using CampusQuest.Mobile.Views;

namespace CampusQuest.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the login page route
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        }
    }
}
