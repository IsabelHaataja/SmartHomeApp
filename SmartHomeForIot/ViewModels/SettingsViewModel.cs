
using Communications.Azure.Email;
using Communications.Azure;
using Communications.gRPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Resources.Data;
using System.Diagnostics;

namespace SmartHomeForIot.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly AzureResourceManager _azureRM;
    private readonly EmailCommunication _email;
    private readonly GrpcManager _grpcManager;
    private readonly DatabaseService _database;

    public SettingsViewModel(DatabaseService database, AzureResourceManager azureRM, EmailCommunication email, GrpcManager grpcManager)
    {
        _database = database;
        _azureRM = azureRM;
        _email = email;
        _grpcManager = grpcManager;
        try
        {
            var settings = _database.GetSettingsAsync().Result;
            if (settings != null)
            {
                IsConfigured = true;
                EmailAddress = settings.EmailAddress;

            }

            ConfigureButtonText = IsConfigured ? "Configured" : "Configure";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex, "Error in SettingsViewModel constructor.");
        }        
    }

    [ObservableProperty]
    private bool isConfigured = false;

    [ObservableProperty]
    private string configureButtonText = "Configure";

    [ObservableProperty]
    private string emailAddress = null!;

    [RelayCommand]
    public async Task Configure()
    {
        try
        {
            ConfigureButtonText = "Configuring...";

            await _azureRM.InitializeAsync();

            IsConfigured = await ConfigureSettingsAsync();

            if (IsConfigured)
                _email.Send(EmailAddress, "Azure IotHub Resources Created", "<h1>Your Azure IotHub was created successfully!</h1>", "Your Azure IotHub was created successfully!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public async Task<bool> ConfigureSettingsAsync()
    {
        try
        {
            var settings = await _database.GetSettingsAsync();
            if (settings == null) 
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
                    await _azureRM.CreateIotHubAsync($"iothub-{settings.AppId}", "westeurope", "F1");
                    iotHub = await _azureRM.GetIotHubInfoAsync();
                }

                settings.IotHubConnectionString = iotHub.ConnectionString!;

                var result = await _database.SaveSettingsAsync(settings);

                return result == 1 ? true : false;
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return false;
        }
        return false;
    }
}
