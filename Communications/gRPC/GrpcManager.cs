using Communications.gRPC.Service;
using Grpc.Core;

namespace Communications.gRPC;

public class GrpcManager
{
    private readonly IotHubGrpcService _iotHubGrpcService;

    public GrpcManager(IotHubGrpcService otHubGrpcService)
    {
        _iotHubGrpcService = otHubGrpcService;
    }

    public async Task StartAsync(string connectionString)
    {
        const int Port = 50051;
        Server server = new()
        {
            Services = { IotService.BindService(_iotHubGrpcService)},
            Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
        };

        server.Start();
        await Task.Delay(Timeout.Infinite);
    }
}
