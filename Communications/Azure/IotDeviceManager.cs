using Communications.Interfaces;
using Microsoft.Azure.Devices;

namespace Communications.Azure;

public class IotDeviceManager : IIotDeviceManager
{
    private static RegistryManager registryManager;

    public IotDeviceManager()
    {
        registryManager = null!;
    }

    public async Task DeleteDeviceFromIoTHubAsync(string deviceId, string iotHubConnectionString)
    {
        if (string.IsNullOrEmpty(deviceId))
            throw new ArgumentException("Device ID cannot be null or empty.");

        if (string.IsNullOrEmpty(iotHubConnectionString))
            throw new ArgumentException("IoT Hub connection string cannot be null or empty.");

        try
        {
            using (registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString))
            {
                Console.WriteLine($"Deleting device with ID: {deviceId} from IoT Hub...");
                await registryManager.RemoveDeviceAsync(deviceId);
                Console.WriteLine($"Device {deviceId} deleted successfully from IoT Hub.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting device from IoT Hub: {ex.Message}");
            throw;
        }
    }
}
