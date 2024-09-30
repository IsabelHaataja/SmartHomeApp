using Communications.Azure.Email;
using Communications.Azure;
using Communications.gRPC.Service;
using Communications.gRPC;
using Microsoft.Extensions.Logging;
using Smart_Home.Pages;
using Smart_Home.ViewModels;

namespace Smart_Home
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

            builder.Services.AddTransient<AzureResourceManager>();
            builder.Services.AddTransient<IotHubGrpcService>();
            builder.Services.AddTransient<GrpcManager>();

            // Home-parts could be scoped
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();

            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();

            builder.Services.AddTransient<EmailCommunication>();

            builder.Logging.AddDebug();


            return builder.Build();
        }
    }
}
