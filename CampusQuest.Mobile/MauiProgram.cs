using CampusQuest.Mobile.Views;
using CampusQuest.Mobile.ViewModels;
using ZXing.Net.Maui;
using CampusQuest.Mobile.Services;
using CampusQuest.Mobile.Views;

namespace CampusQuest.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold", "OpenSansSemibold");
            });

       
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MissionService>();
        // Register pages that require constructor injection
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}