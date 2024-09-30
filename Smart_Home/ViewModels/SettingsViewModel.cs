

using Communications.Azure;
using Communications.Azure.Email;
using Communications.gRPC;

namespace Smart_Home.ViewModels;

public class SettingsViewModel
{
    private readonly AzureResourceManager _azureRM;
    private readonly EmailCommunication _email;
    private readonly GrpcManager _grpcManager;

    public SettingsViewModel(AzureResourceManager azureRM, EmailCommunication email, GrpcManager grpcManager)
    {
        _azureRM = azureRM;
        _email = email;
        _grpcManager = grpcManager;
    }
}
