using Resources.Data.Models;

namespace Resources.Data
{
    public interface IDatabaseService
    {
        Task<int> DeleteSettingsAsync(DeviceSettings settings);
        Task<DeviceSettings> GetSettingsAsync();
        Task<int> SaveSettingsAsync(DeviceSettings settings);
    }
}