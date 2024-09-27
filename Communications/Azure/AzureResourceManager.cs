using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Azure;

public class AzureResourceManager
{


    public async Task<ResourceGroupResource> CreateResourceGroupAsync(string resourceGroupName, string location)
    {

        ResourceGroupCollection resourceGroups = 
    }

    public AzureLocation GetAzureLocation(string location)
    {
        return location.ToLower() switch
        {
            "westerneurope" = AzureLocation.WestEurope,

        }
    }
}
