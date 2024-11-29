using Communications.Interfaces;
using Microsoft.Azure.Devices;
using System.Diagnostics;
using System.Text;

namespace Communications.Azure;

public class IotHubService : IIotHubService
{
    private readonly string _iotHubConnectionString;
    private readonly ServiceClient _serviceClient;

    public IotHubService(string iotHubConnectionString)
    {
        _iotHubConnectionString = EnsureValidHostName(iotHubConnectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubConnectionString);

        Debug.WriteLine($"{_iotHubConnectionString}");
    }

    // Rebuild connectionstring - add .azure-devices.net to hostname
    private string EnsureValidHostName(string connectionString)
    {
        var connectionStringParts = connectionString.Split(';');
        for (int i = 0; i < connectionStringParts.Length; i++)
        {
            if (connectionStringParts[i].StartsWith("HostName="))
            {
                var hostName = connectionStringParts[i].Substring("HostName=".Length);
                if (!hostName.Contains(".azure-devices.net"))
                {
                    connectionStringParts[i] = $"HostName={hostName}.azure-devices.net";
                }
            }
        }

        return string.Join(";", connectionStringParts);
    }

    //Direct method
    public async Task InvokeDeviceMethodAsync(string deviceId, string methodName)
    {
        try
        {
            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = TimeSpan.FromSeconds(30),
                ConnectionTimeout = TimeSpan.FromSeconds(30)
            };

            var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

            Console.WriteLine($"Method {methodName} invoked on device {deviceId}, Response: {response.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to invoke method {methodName}: {ex.Message}");
        }
    }

    // C2D
    public async Task SendCloudToDeviceMessageAsync(string deviceId, string message)
    {
        var commandMessage = new Message(Encoding.ASCII.GetBytes(message))
        {
            ExpiryTimeUtc = DateTime.UtcNow.AddMinutes(10)
        };

        try
        {
            await _serviceClient.SendAsync(deviceId, commandMessage);

            Console.WriteLine($"C2D message sent to device {deviceId}: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send C2D message: {ex.Message}");
        }
    }
}
