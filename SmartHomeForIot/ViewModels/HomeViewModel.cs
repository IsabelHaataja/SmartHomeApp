
using Communications.Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Resources.Data;
using System.Diagnostics;

namespace SmartHomeForIot.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IDatabaseService _context;
    private IotHubService? _iotHubService;

    [ObservableProperty]
    private string deviceState = "Off";

    [ObservableProperty]
    private string toggleButtonText = "Turn On";

    public HomeViewModel(IDatabaseService context, IotHubService iotHubService)
    {
        _context = context;
        _iotHubService = iotHubService;
        InitializeIotHubServiceAsync().ConfigureAwait(false);
        UpdateToggleButtonText();

    }

    [RelayCommand]
    private async Task ToggleDeviceStateAsync()
    {
        //if (_iotHubService == null)
        //{
        //    await InitializeIotHubServiceAsync();
        //}

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

    // Simulate sending the command to the IoT device (this would invoke your IoT Hub logic)
    private async Task SendDeviceCommandAsync(string command)
    {
        //if (_iotHubService == null)
        //{
        //    // Initialize IotHubService if it's not already done
        //    await InitializeIotHubServiceAsync();
        //}

        if (_iotHubService != null)
        {
            //TODO - add device id dynamically
            await _iotHubService.InvokeDeviceMethodAsync("AC-45ffebf0", command);

            // Simulate a delay (optional)
            await Task.Delay(500);
        }
        else
        {
            Debug.WriteLine("Error: IoT Hub service is not initialized.");
        }
    }
    private async Task InitializeIotHubServiceAsync()
    {
        try
        {
            var deviceSettings = await _context.GetSettingsAsync(); // Assumes a method to fetch DeviceSettings

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
