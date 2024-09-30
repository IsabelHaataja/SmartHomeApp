using Resources.Data.Models;
using SQLite;

namespace Resources.Data;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;
    
    public DatabaseService()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Smarthome_database");
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
