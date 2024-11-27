
using Communications.Azure.Email;
using Communications.Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Resources.Data;
using System.Diagnostics;
using Communications.Interfaces;
using DataModels = Resources.Data.Models;

namespace SmartHomeForIot.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly AzureResourceManager _azureRM;
    private readonly EmailCommunication _email;
    private readonly IDatabaseService _database;

    public SettingsViewModel(IDatabaseService database, AzureResourceManager azureRM, EmailCommunication email)
    {
        _database = database;
        _azureRM = azureRM;
        _email = email;

        InitializeSettingsAsync();
    }

    [ObservableProperty]
    private bool isConfigured = false;

    [ObservableProperty]
    private string configureButtonText = "Configure";

    [ObservableProperty]
    private string emailAddress = null!;

    [ObservableProperty]
    private string deviceId = string.Empty;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string? iotHubConnectionString;

    [ObservableProperty]
    private string? deviceConnectionString;

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

    [RelayCommand]
    public async Task DeleteDevice()
    {
        try
        {
            if (IsConfigured && !string.IsNullOrEmpty(DeviceId))
            { 
                var settings = await _database.GetSettingsAsync();

                if (settings != null && settings.Id == DeviceId)
                {
                    await _database.DeleteSettingsAsync(DeviceId);

                    var deviceManager = new IotDeviceManager();

                    await deviceManager.DeleteDeviceFromIoTHubAsync(DeviceId, settings.IotHubConnectionString);

                    _email.Send(EmailAddress, $"Device with ID: {DeviceId} Deleted", "<h1>Your device has been successfully deleted!</h1>", "Your device has been deleted.");

                    IsConfigured = false;
                    ConfigureButtonText = "Configure";
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex, "Error deleting the device.");
        }
    }

    [RelayCommand]
    public void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
    }

    [RelayCommand]
    public async Task SaveUpdatedSettings()
    {
        try
        {
            var settings = await _database.GetSettingsAsync();
            if (settings != null)
            {
                settings.IotHubConnectionString = IotHubConnectionString;
                settings.DeviceConnectionString = DeviceConnectionString;
                await _database.SaveSettingsAsync(settings);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving updated settings: {ex.Message}");
        }
    }

    [RelayCommand]
    public void CancelEdit()
    {
        IsEditMode = false;

        InitializeSettingsAsync();
    }

    private async void InitializeSettingsAsync()
    {
        try
        {
            var settings = await _database.GetSettingsAsync();
            if (settings != null)
            {
                IsConfigured = true;
                EmailAddress = settings.EmailAddress;
                IotHubConnectionString = settings.IotHubConnectionString;
                DeviceConnectionString = settings.DeviceConnectionString;
            }

            ConfigureButtonText = IsConfigured ? "Configured" : "Configure";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex, "Error in SettingsViewModel InitializeSettingsAsync.");
        }
    }
    public async Task<bool> ConfigureSettingsAsync()
    {
        try
        {
            Debug.WriteLine("Checking existing settings...");
            var settings = await _database.GetSettingsAsync();
            if (settings == null) 
            {
                Debug.WriteLine("No existing settings found. Creating new settings...");
                settings = new DataModels.DeviceSettings
                {
                    Id = Guid.NewGuid().ToString().Split('-')[0],
                    EmailAddress = EmailAddress
                };

                var iotHub = await _azureRM.GetIotHubInfoAsync();

                if (iotHub == null)
                {
                    await _azureRM.CreateResourceGroupAsync($"rg-{settings.Id}", "westeurope");
                    await _azureRM.CreateIotHubAsync($"iothub-{settings.Id}", "westeurope", "F1");
                    iotHub = await _azureRM.GetIotHubInfoAsync();
                }
                Debug.WriteLine($"Generated new ID: {settings.Id} for Email: {EmailAddress}");
                settings.IotHubConnectionString = iotHub.ConnectionString!;
                Debug.WriteLine($"IoT Hub Connection String: {settings.IotHubConnectionString}");

                var result = await _database.SaveSettingsAsync(settings);
                Debug.WriteLine($"Save result: {result}");

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
