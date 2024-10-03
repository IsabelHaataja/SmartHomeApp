using Communications.Azure.Email;
using Communications.Azure;
using Communications.gRPC.Service;
using Communications.gRPC;
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
                    fonts.AddFont("fa-regular-400.ttf", "fa-regular");
                    fonts.AddFont("fa-solid-900.ttf", "fa-solid");
                    fonts.AddFont("fa-brands-400.ttf", "fa-brands");
                    fonts.AddFont("Poppins-Regular.ttf", "Poppins-Regular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "Poppins-SemiBold");
                    fonts.AddFont("Poppins-Thin.ttf", "Poppins-Thin");
                });


            builder.Services.AddTransient<AzureResourceManager>();
            builder.Services.AddTransient<IotHubGrpcService>();
            builder.Services.AddTransient<GrpcManager>();
            builder.Services.AddTransient<EmailCommunication>();

            builder.Services.AddTransient<DatabaseService>();

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
