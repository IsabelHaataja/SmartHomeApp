
using Azure.Communication.Email;
using Azure;
using System.Diagnostics;


namespace Communications.Azure.Email;

public class EmailCommunication
{
    private readonly string _senderAddress;
    private readonly EmailClient _client;

    public EmailCommunication(string connectionString = "endpoint=https://cs-f9d56587.europe.communication.azure.com/;accesskey=CQkhvdwAOXkS4ij430nGjgQV6AFPny0k6t6ckEttxCxkqAhh8OciJQQJ99AIACULyCp3CzXbAAAAAZCSfJdY", string senderAddress = "DoNotReply@8abdee84-8838-400b-b09a-0263d8d33c27.azurecomm.net")
    {

            _client = new EmailClient(connectionString);
            _senderAddress = senderAddress;
    }

    public void Send(string toAddress, string subject, string body, string plainTextBody)
    {
        try
        {
            Debug.WriteLine($"Sending email to {toAddress} with subject {subject}.");

            EmailSendOperation emailSendOperation = _client.Send(
                WaitUntil.Completed,
                senderAddress: _senderAddress,
                recipientAddress: toAddress,
                subject: subject,
                htmlContent: body,
                plainTextContent: plainTextBody
            );
            
            Debug.WriteLine("Email sent successfully.");
        }
        catch (Exception ex) 
        { 
            Debug.WriteLine(ex, "Failed to send email.");
            Debug.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}
