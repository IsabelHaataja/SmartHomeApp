namespace Resources.Data.Models;

public class Settings
{
    // TODO - Change class name to DeviceSettings
    public string AppId { get; set; } = null!;
    public string IotHubConnectionString { get; set; } = null!;
    public string EmailAddress {  get; set; } = null!;

    //    public string? Type { get; set; }
}
