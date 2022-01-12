namespace AzureServices.Services.Storage.Queues;

public interface IQueueService
{
    Task<string> ClearMessagesAsync();
    Task<string> DeleteMessageAsync(string name);
    Task<string> PeekMessageAsync();
    Task<string> PeekMessagesAsync();
    Task<string> ReceiveMessageAsync();
    Task<string> ReceiveMessagesAsync();
    Task<string> SendMessageAsync(string messageText);
    Task<string> UpdateMessageAsync(string messageId, string popReceipt, string message);
}