using Communications.Azure.Email;
using Communications.Azure;
using Microsoft.Extensions.Logging;
using SmartHomeForIot.Pages;
using SmartHomeForIot.ViewModels;
using Resources.Data;

namespace SmartHomeForIot
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //builder.Services.AddLogging();

            builder.Services.AddTransient<AzureResourceManager>();
            builder.Services.AddTransient<EmailCommunication>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // Home-parts could be scoped
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();

            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<SettingsViewModel>();

            builder.Logging.AddDebug();

            return builder.Build();
        }
    }
}
