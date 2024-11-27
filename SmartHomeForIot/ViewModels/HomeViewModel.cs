
using Communications.Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Resources.Data;
using System.Diagnostics;

namespace SmartHomeForIot.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IDatabaseService _context;
    private readonly SettingsViewModel _settingsViewModel;
    private IotHubService? _iotHubService;

    [ObservableProperty]
    private string deviceState = "Off";

    [ObservableProperty]
    private string toggleButtonText = "Turn On";

    [ObservableProperty]
    private string? deviceId;

    [ObservableProperty]
    private string? deviceType;

    [ObservableProperty]
    private bool isConfigured;

    public HomeViewModel(IDatabaseService context, SettingsViewModel settingsViewModel)
    {
        _context = context;
        _settingsViewModel = settingsViewModel;

        InitializeDeviceInfoAsync().ConfigureAwait(false);
        UpdateToggleButtonText();

    }

    private async Task InitializeDeviceInfoAsync()
    {
        try
        {
            DeviceId = await _context.GetDeviceIdFromConnectionStringAsync();
            DeviceType = await _context.GetDeviceTypeAsync();
            IsConfigured = _settingsViewModel.IsConfigured;
        }
        catch (Exception ex)
        {
        }
    }


    [RelayCommand]
    private async Task ToggleDeviceStateAsync()
    {
        if (_iotHubService == null)
        {
            await InitializeIotHubServiceAsync();
        }

        if (_iotHubService != null)
        {
            if (DeviceState == "On")
            {
                await SendDeviceCommandAsync("TurnOff");
                DeviceState = "Off";
            }
            else
            {
                await SendDeviceCommandAsync("TurnOn");
                DeviceState = "On";
            }

            UpdateToggleButtonText();
        }
        else
        {
            Debug.WriteLine("Error: Failed to initialize IoT Hub service.");
        }
    }
    private void UpdateToggleButtonText()
    {
        ToggleButtonText = DeviceState == "On" ? "Turn Off" : "Turn On";
    }

    private async Task SendDeviceCommandAsync(string command)
    {

        if (_iotHubService != null)
        {
            var deviceId = await _context.GetDeviceIdFromConnectionStringAsync();
            //await _iotHubService.InvokeDeviceMethodAsync(deviceId, command);
            if (deviceId != null)
            {
                await _iotHubService.SendCloudToDeviceMessageAsync(deviceId, command);
                await Task.Delay(500);
            }
            else
            {
                Debug.WriteLine("Error: DeviceId not found in database.");
            }
        }
        else
        {
            Debug.WriteLine("Error: IoT Hub service is not initialized.");
        }
    }
    private async Task InitializeIotHubServiceAsync()
    {
        if (_iotHubService == null)
        {
            try
            {
                var deviceSettings = await _context.GetSettingsAsync();

                if (deviceSettings != null && !string.IsNullOrEmpty(deviceSettings.IotHubConnectionString))
                {
                    _iotHubService = new IotHubService(deviceSettings.IotHubConnectionString);
                    Debug.WriteLine("IotHubService initialized successfully.");
                }
                else
                {
                    Debug.WriteLine("Error: IoT Hub connection string is missing or invalid.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not initialize iothubservice: {ex.Message}");
            }
        }
    }
}
