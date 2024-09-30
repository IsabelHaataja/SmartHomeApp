
using Communications.Azure.Email;
using Communications.Azure;
using Communications.gRPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Resources.Data;

namespace SmartHomeForIot.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly AzureResourceManager _azureRM;
    private readonly EmailCommunication _email;
    private readonly GrpcManager _grpcManager;
    private readonly DatabaseService _database;

    public SettingsViewModel(AzureResourceManager azureRM, EmailCommunication email, GrpcManager grpcManager)
    {
        _azureRM = azureRM;
        _email = email;
        _grpcManager = grpcManager;
    }

    [ObservableProperty]
    private bool isConfigured = false;

    [ObservableProperty]
    private string emailAddress = null!;

    [RelayCommand]
    public async Task Configure()
    {
        IsConfigured = await ConfigureSettingsAsync();
    }

    public async Task<bool> ConfigureSettingsAsync()
    {
        try
        {
            var settings = await _database.GetSettingsAsync();
            if (settings != null) 
            {
                settings = new Resources.Data.Models.Settings
                {
                    AppId = Guid.NewGuid().ToString().Split('-')[0],
                    EmailAddress = EmailAddress
                };

                var iotHub = await _azureRM.GetIotHubInfoAsync();

                if (iotHub == null)
                {
                    await _azureRM.CreateResourceGroupAsync($"rg-{settings.AppId}", "westeurope");
                    await _azureRM.CreateIotHubAsync($"IOTHUB-{settings.AppId}", "westeurope", "F1");
                    iotHub = await _azureRM.GetIotHubInfoAsync();
                }

                settings.IotHubConnectionString = iotHub.ConnectionString;

                await _database.SaveSettingsAsync(settings);

                return true;
            }
        }
        catch 
        {
            return false;
        }
    }
}
