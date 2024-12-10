using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.IotHub;
using Azure.ResourceManager.IotHub.Models;
using Azure.ResourceManager.Resources;
using Communications.Azure.Models;
using Communications.Interfaces;
using System.Diagnostics;


namespace Communications.Azure;

public class AzureResourceManager : IAzureResourceManager
{
    ArmClient _client = null!;
    SubscriptionResource _subscription = null!;
    ResourceGroupResource? _currentResourceGroup;
    IotHubDescriptionResource? _currentIotHub;
    public async Task InitializeAsync()
    {
        try
        {
            var credential = new AzureCliCredential();
            _client = new ArmClient(credential);

            _subscription = await _client.GetDefaultSubscriptionAsync();

            if (_subscription == null)
            {
                throw new Exception("No subscriptions found for the given credentials.");
            }
            Debug.WriteLine($"Successfully authenticated with subscription: {_subscription.Data.DisplayName}");
        }
        catch (CredentialUnavailableException ex)
        {
            Debug.WriteLine($"CredentialUnavailableException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred in Initialization of default subscription", ex);
        }
    }

    public async Task<ResourceGroupResource> CreateResourceGroupAsync(string resourceGroupName, string location)
    {
        try
        {
            ResourceGroupCollection resourceGroups = _subscription.GetResourceGroups();
            ResourceGroupData resourceGroupData = new(GetAzureLocation(location));
            ArmOperation<ResourceGroupResource> operation =
                await resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, resourceGroupData);

            _currentResourceGroup = operation.Value;

            return operation.Value;
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return null!;
        }
    }

    public async Task<IotHubDescriptionResource> CreateIotHubAsync(string iotHubUniqueName, string location, string sku)
    {
        try
        {
            IotHubDescriptionCollection iotHubDescriptionCollection = _currentResourceGroup.GetIotHubDescriptions();
            IotHubDescriptionData iotHubDescriptionData = new(location, new IotHubSkuInfo(GetIotHubSku(sku)) { Capacity = 1 });
            ArmOperation<IotHubDescriptionResource> operation =
                await iotHubDescriptionCollection.CreateOrUpdateAsync(WaitUntil.Completed, iotHubUniqueName, iotHubDescriptionData);

            _currentIotHub = operation.Value;

            return operation.Value;
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return null!;
        }

    }

    public async Task<IotHubDescriptionResource> CreateIotHubAsync(ResourceGroupResource resourceGroup, string iotHubUniqueName, string location, string sku)
    {
        try
        {
            IotHubDescriptionCollection iotHubDescriptionCollection = resourceGroup.GetIotHubDescriptions();
            IotHubDescriptionData iotHubDescriptionData = new(location, new IotHubSkuInfo(GetIotHubSku(sku)) { Capacity = 1 });
            ArmOperation<IotHubDescriptionResource> operation =
                await iotHubDescriptionCollection.CreateOrUpdateAsync(WaitUntil.Completed, iotHubUniqueName, iotHubDescriptionData);

            _currentIotHub = operation.Value;

            return operation.Value;
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return null!;
        }

    }

    public async Task<IotHubKey> GetIotHubInfoAsync(string keyName = "iothubowner")
    {
        try
        {
            if (_currentIotHub != null)
            {
                var result = await _currentIotHub.GetKeysForKeyNameAsync(keyName);
                var value = result.Value;

                var iotHubKey = new IotHubKey
                {
                    HostName = _currentIotHub.Data.Name,
                    SharedAccessKeyName = value.KeyName,
                    SharedAccessKey = value.PrimaryKey
                };

                return iotHubKey;
            }
            return null!;

        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return null!;
        }
    }
    public async Task<IotHubKey> GetIotHubConnectionString(IotHubDescriptionResource iotHub, string keyName = "iothubowner")
    {
        try
        {
            var result = await iotHub.GetKeysForKeyNameAsync(keyName);
            var value = result.Value;

            var iotHubKey = new IotHubKey
            {
                HostName = $"{iotHub.Data.Name}.azure-devices.net",
                SharedAccessKeyName = value.KeyName,
                SharedAccessKey = value.PrimaryKey
            };

            return iotHubKey;
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return null!;
        }
    }

    public AzureLocation GetAzureLocation(string location)
    {
        return location.ToLower() switch
        {
            "westeurope" => AzureLocation.WestEurope,
            "northeurope" => AzureLocation.NorthEurope,
            "swedencentral" => AzureLocation.SwedenCentral,
            _ => location = AzureLocation.WestEurope
        };
    }
    public IotHubSku GetIotHubSku(string sku)
    {
        return sku.ToLower() switch
        {
            "F1" => IotHubSku.F1,
            "S1" => IotHubSku.S1,
            _ => IotHubSku.S1
        };
    }
}
