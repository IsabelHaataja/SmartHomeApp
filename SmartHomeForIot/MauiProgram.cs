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
                });

            //builder.Services.AddLogging();

            builder.Services.AddTransient<AzureResourceManager>();
            builder.Services.AddTransient<IotHubGrpcService>();
            builder.Services.AddTransient<GrpcManager>();
            builder.Services.AddTransient<EmailCommunication>();
            builder.Services.AddTransient<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IotHubService>(serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<IDatabaseService>();
                var settingsTask = dbContext.GetSettingsAsync();
                var settings = settingsTask.Result;

                if (settings != null)
                {
                    return new IotHubService(settings.IotHubConnectionString);
                }
                throw new InvalidOperationException("COuld not retreive iotHubCOnnectionString from the db.");
            });

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
