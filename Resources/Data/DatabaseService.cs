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
    private readonly string _dbFolder;
    private readonly string _dbPath;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        _dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmarthomeDatabase");

        if (!Directory.Exists(_dbFolder))
        {
            Directory.CreateDirectory(_dbFolder);
        }

        _dbPath = Path.Combine(_dbFolder, "Smarthome_database.db3");

        _database = new SQLiteAsyncConnection(_dbPath);
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

            _logger.LogInformation("DeviceSettings table created successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create DeviceSettings table: {ex.Message}");
            _logger.LogError(ex, "Failed to create DeviceSettings table.");
        }
    }

    public async Task<string> GetIotHubConnectionStringAsync()
    {
        try
        {
            // Fetch settings from the database
            var response = await GetSettingsAsync();
            var connectionString = response?.IotHubConnectionString ?? string.Empty;

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("IoT Hub connection string is empty or null.");
                return string.Empty;
            }

            // Extract HostName from the connection string
            var hostNamePart = connectionString.Split(';')
                .FirstOrDefault(part => part.StartsWith("HostName=", StringComparison.OrdinalIgnoreCase));

            if (hostNamePart == null)
            {
                Debug.WriteLine("HostName is missing in the IoT Hub connection string.");
                return string.Empty;
            }

            // Extract the actual HostName value
            var hostName = hostNamePart.Split('=')[1];

            // Check if HostName ends with ".azure-devices.net", and append if missing
            if (!hostName.EndsWith(".azure-devices.net", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine($"HostName before fix: {hostName}");

                // Append ".azure-devices.net" to the HostName
                hostName += ".azure-devices.net";

                // Rebuild the connection string with the corrected HostName
                var updatedHostNamePart = $"HostName={hostName}";
                connectionString = connectionString.Replace(hostNamePart, updatedHostNamePart);

                Debug.WriteLine($"HostName after fix: {hostName}");
            }

            Debug.WriteLine($"Final IoT Hub connection string: {connectionString}");

            return connectionString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the IoT Hub connection string.");
            return string.Empty;
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
        var settings = await _database.Table<DeviceSettings>()
                               .Where(s => s.Id == deviceId)
                               .FirstOrDefaultAsync();

        if (settings != null)
        {
            settings.Id = null;
            settings.Type = null;
            settings.EmailAddress = null;
            settings.DeviceConnectionString = null;
            settings.IotHubConnectionString = null;

            return await _database.DeleteAsync(settings);  
        }

        return 0;
    }
    public async Task DeleteDatabaseAndFolderAsync()
    {
        try
        {
            // Close SQLite connection
            if (_database != null)
            {
                await _database.CloseAsync();
                Debug.WriteLine("Database connection closed.");
            }

            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
                Debug.WriteLine("Database file deleted.");
            }

            if (Directory.Exists(_dbFolder))
            {
                Directory.Delete(_dbFolder, true);
                Debug.WriteLine("Database folder deleted.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting database or folder: {ex.Message}");
        }
    }
}
