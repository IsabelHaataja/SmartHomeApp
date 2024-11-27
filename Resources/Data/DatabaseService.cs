using Resources.Data.Models;
using SQLite;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.AccessControl;
using System.IO;

namespace Resources.Data;

public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private SQLiteAsyncConnection _database;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        string dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmarthomeDatabase");

        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }

        string dbPath = Path.Combine(dbFolder, "Smarthome_database.db3");
        Debug.WriteLine($"Database Path: {dbPath}");

        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        await CreateDeviceSettingsTableAsync();
    }
    private async Task CreateDeviceSettingsTableAsync()
    {
        try
        {
            var result = await _database.CreateTableAsync<DeviceSettings>();

            Debug.WriteLine($"Table creation result: {result}");

            Debug.WriteLine("DeviceSettings table created successfully.");
            _logger.LogInformation("DeviceSettings table created successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create DeviceSettings table: {ex.Message}");
            _logger.LogError(ex, "Failed to create DeviceSettings table.");
        }
    }
    public async Task<string?> GetDeviceIdFromConnectionStringAsync()
    {
        try
        {
            var settings = await GetSettingsAsync();
            var deviceConnectionString = settings.DeviceConnectionString;

            if (string.IsNullOrEmpty(deviceConnectionString))
            {
                Console.WriteLine("Device connection string is missing in the database.");
                return null;
            }
            var deviceIdPart = deviceConnectionString.Split(';')
                .FirstOrDefault(part => part.StartsWith("DeviceId=", StringComparison.OrdinalIgnoreCase));

            if (deviceIdPart == null)
            {
                Console.WriteLine("DeviceId not found in the device connection string.");
                return null;
            }

            var deviceId = deviceIdPart.Split('=')[1];
            return deviceId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving the DeviceId: {ex.Message}");
            return null;
        }
    }
    public async Task<string?> GetDeviceTypeAsync()
    {
        var settings = await GetSettingsAsync();
        var deviceType = settings.Type;
        return deviceType;
    }
    public Task<DeviceSettings> GetSettingsAsync()
    {
        return _database.Table<DeviceSettings>().FirstOrDefaultAsync();
    }

    public Task<int> SaveSettingsAsync(DeviceSettings settings)
    {
        return _database.InsertOrReplaceAsync(settings);
    }
    public async Task<int> DeleteSettingsAsync(string deviceId)
    {
        // TODO - välj vad som ska tas bort, inte iothub connectionstring iaf
        var settings = await _database.Table<DeviceSettings>()
                               .Where(s => s.Id == deviceId)
                               .FirstOrDefaultAsync();

        if (settings != null)
        {
            return await _database.DeleteAsync(settings);  
        }

        return 0;
    }
}
