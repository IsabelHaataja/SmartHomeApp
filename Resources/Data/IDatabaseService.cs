using Resources.Data.Models;

namespace Resources.Data
{
    public interface IDatabaseService
    {
        Task<int> DeleteSettingsAsync(string deviceId);
        Task<DeviceSettings> GetSettingsAsync();
        Task<int> SaveSettingsAsync(DeviceSettings settings);
        Task<string> GetIotHubConnectionStringAsync();
        Task<string?> GetDeviceIdFromConnectionStringAsync();
        Task<string?> GetDeviceTypeAsync();
    }
}