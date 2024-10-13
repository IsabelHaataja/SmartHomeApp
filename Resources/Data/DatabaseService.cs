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

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Smarthome_database.db3");
        Debug.WriteLine($"Database Path: {dbPath}");

        _database = new SQLiteAsyncConnection(dbPath);
        CreateDeviceSettingsTableAsync().ConfigureAwait(false);
    }
    private async Task CreateDeviceSettingsTableAsync()
    {
        try
        {
            // Await the table creation task
            var result = await _database.CreateTableAsync<DeviceSettings>();

            Debug.WriteLine($"Table creation result: {result}");
            // Log to output when table creation is successful
            Debug.WriteLine("DeviceSettings table created successfully.");
            _logger.LogInformation("DeviceSettings table created successfully.");
        }
        catch (Exception ex)
        {
            // Log any exception during table creation
            Debug.WriteLine($"Failed to create DeviceSettings table: {ex.Message}");
            _logger.LogError(ex, "Failed to create DeviceSettings table.");
        }
    }
    public Task<DeviceSettings> GetSettingsAsync()
    {
        return _database.Table<DeviceSettings>().FirstOrDefaultAsync();
    }

    public Task<int> SaveSettingsAsync(DeviceSettings settings)
    {
        return _database.InsertOrReplaceAsync(settings);
    }
    public Task<int> DeleteSettingsAsync(DeviceSettings settings)
    {
        return _database.DeleteAsync(settings);
    }
}
