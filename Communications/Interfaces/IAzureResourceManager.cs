using Azure.Core;
using Azure.ResourceManager.IotHub;
using Azure.ResourceManager.IotHub.Models;
using Azure.ResourceManager.Resources;
using Communications.Azure.Models;

namespace Communications.Interfaces
{
    public interface IAzureResourceManager
    {
        Task<IotHubDescriptionResource> CreateIotHubAsync(ResourceGroupResource resourceGroup, string iotHubUniqueName, string location, string sku);
        Task<IotHubDescriptionResource> CreateIotHubAsync(string iotHubUniqueName, string location, string sku);
        Task<ResourceGroupResource> CreateResourceGroupAsync(string resourceGroupName, string location);
        Task DeleteDeviceAsync(string id);
        AzureLocation GetAzureLocation(string location);
        Task<IotHubKey> GetIotHubConnectionString(IotHubDescriptionResource iotHub, string keyName = "iothubowner");
        Task<IotHubKey> GetIotHubInfoAsync(string keyName = "iothubowner");
        IotHubSku GetIotHubSku(string sku);
        Task InitializeAsync();
    }
}