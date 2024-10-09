using Resources.Data.Models;
using SQLite;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Resources.Data;

public class DatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private SQLiteAsyncConnection _database;
    
    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Smarthome_database.db3");
        Debug.WriteLine($"Database Path: {dbPath}");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Settings>().Wait();
    }

    public Task<Settings> GetSettingsAsync()
    {
        return _database.Table<Settings>().FirstOrDefaultAsync();
    }

    public Task<int> SaveSettingsAsync(Settings settings)
    {
        return _database.InsertOrReplaceAsync(settings);
    }
    public Task<int> DeleteSettingsAsync(Settings settings)
    {
        return _database.DeleteAsync(settings);
    }
}
