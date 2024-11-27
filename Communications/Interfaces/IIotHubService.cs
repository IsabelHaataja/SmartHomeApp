namespace Communications.Interfaces
{
    public interface IIotHubService
    {
        Task InvokeDeviceMethodAsync(string deviceId, string methodName);
        Task SendCloudToDeviceMessageAsync(string deviceId, string message);
    }
}