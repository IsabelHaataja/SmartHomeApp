
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
    private readonly IIotDeviceManager _iotDeviceManager;

    public SettingsViewModel(IDatabaseService database, AzureResourceManager azureRM, EmailCommunication email, IIotDeviceManager iotDeviceManager)
    {
        _database = database;
        _azureRM = azureRM;
        _email = email;
        _iotDeviceManager = iotDeviceManager;

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

    private string _statusMessage;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

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

            ConfigureButtonText = "Configured";
            IsConfigured = true;
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
            if (IsConfigured)
            {
                // Retrieve the DeviceId from the connection string
                var deviceId = await _database.GetDeviceIdFromConnectionStringAsync();

                if (string.IsNullOrEmpty(deviceId))
                {
                    Debug.WriteLine("DeviceId is null or empty. Aborting deletion.");
                    return;
                }

                var iotHubConnectionString = await _database.GetIotHubConnectionStringAsync();

                if (string.IsNullOrEmpty(iotHubConnectionString))
                {
                    Debug.WriteLine("IoT Hub connection string is null or empty. Aborting deletion.");
                    return;
                }

                await _iotDeviceManager.DeleteDeviceFromIoTHubAsync(deviceId, iotHubConnectionString);

                // Retrieve settings from the database
                var settings = await _database.GetSettingsAsync();
                if (settings != null)
                {
                    await _database.DeleteSettingsAsync(settings.Id);
                }

                _database.DeleteDatabaseAndFolderAsync();

                _email.Send(EmailAddress, $"Device with ID: {deviceId} Deleted", "<h1>Your device has been successfully deleted!</h1>", "Your device has been deleted.");

                // Reset state
                IsConfigured = false;
                ConfigureButtonText = "Configure";

                StatusMessage = "Device deleted. Restart app to configure device.";
            }
            else
            {
                Debug.WriteLine("System is not configured. Aborting deletion.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting the device: {ex.Message}");
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
                settings.EmailAddress = EmailAddress;
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
                DeviceId = await _database.GetDeviceIdFromConnectionStringAsync();
                EmailAddress = settings.EmailAddress;
                IotHubConnectionString = await _database.GetIotHubConnectionStringAsync();
                DeviceConnectionString = settings.DeviceConnectionString;
            }

            ConfigureButtonText = IsConfigured ? "Configured" : "Configure";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex, "Error initializing settings.");
        }
    }
    public async Task<bool> ConfigureSettingsAsync()
    {
        try
        {
            var settings = await _database.GetSettingsAsync();
            if (settings == null) 
            {
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
