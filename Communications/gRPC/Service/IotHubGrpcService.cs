using Communications.Azure;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Communications.gRPC.Service;

public class IotHubGrpcService : IotService.IotServiceBase
{
    private AzureResourceManager _azureRM;

    public IotHubGrpcService(AzureResourceManager azureRM)
    {
        _azureRM = azureRM;
    }

    public override async Task<IotHubInfoResponse> GetIotHubInfo(IotHubInfoRequest request, ServerCallContext context)
    {
        var result = await _azureRM.GetIotHubInfoAsync();

        return new IotHubInfoResponse
        {
            HostName = result.HostName
        };
    }
}
