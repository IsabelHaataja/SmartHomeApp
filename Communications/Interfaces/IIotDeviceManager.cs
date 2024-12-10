namespace Communications.Interfaces
{
    public interface IIotDeviceManager
    {
        Task DeleteDeviceFromIoTHubAsync(string deviceId, string iotHubConnectionString);
    }
}