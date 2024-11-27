using Communications.Interfaces;
using Microsoft.Azure.Devices;

namespace Communications.Azure;

public class IotDeviceManager : IIotDeviceManager
{
    private static RegistryManager registryManager;
    public async Task DeleteDeviceFromIoTHubAsync(string deviceId, string iotHubConnectionString)
    {
        try
        {
            registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

            Console.WriteLine($"Deleting device with ID: {deviceId} from IoT Hub...");

            await registryManager.RemoveDeviceAsync(deviceId);

            Console.WriteLine($"Device {deviceId} deleted successfully from IoT Hub.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting device from IoT Hub: {ex.Message}");
        }
    }
}
