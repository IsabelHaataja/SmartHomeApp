using Microsoft.Extensions.Logging;
using Communications.gRPC.Service;
using Communications.Azure;
using Communications.gRPC;
using Communications.Azure.Email;
using SmartHome.Pages;
using SmartHome.ViewModels;

namespace SmartHome
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

            return builder.Build();
        }
    }
}
