using Azure;
using Azure.Storage.Queues;
using AzureServices.Services.KeyVault.Secrets;

namespace AzureServices.Services.Storage.Queues;

sealed public class QueueService : AzureService, IQueueService
{
    QueueClient _queueClient;

    public QueueService(IConfiguration configuration, ISecretService secretsService) : base(configuration)
    {
        var connectionString = secretsService.GetSecretValueAsync(ServiceProperties.StorageConnectionString).Result;
        _queueClient = new QueueClient(connectionString, ServiceProperties.QueueName);
    }

    public async Task<string> ClearMessagesAsync()
    {
        try
        {
            await _queueClient.ClearMessagesAsync();
            return "All mesages clear";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> DeleteMessageAsync(string messageId)
    {
        try
        {
            var messageResponse = await _queueClient.ReceiveMessagesAsync();
            var message = messageResponse.Value.Where(x => x.MessageId == messageId).FirstOrDefault();
            if (message != null)
            {
                var deleteResponse = await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                return $"Message with Id - {message.MessageId} was deleted";
            }
            else
            {
                throw new RequestFailedException(404, "MessageId not found");
            }
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> PeekMessageAsync()
    {
        try
        {
            var peekResponse = await _queueClient.PeekMessageAsync();
            var message = peekResponse.Value;
            if (message != null)
            {
                return $"Message Id: {message.MessageId}; " +
                $"ExpiresOn: {message.ExpiresOn}; " +
                $"InsertedOn: {message.InsertedOn}; " +
                $"DequeueCount: {message.DequeueCount}; " +
                $"Message: {message.Body}.";
            }
            else
            {
                throw new RequestFailedException(404, "MessageId not found");
            }

        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> PeekMessagesAsync()
    {
        try
        {
            var response = await _queueClient.PeekMessagesAsync();
            var messageArray = response.Value;
            var result = string.Empty;
            if (messageArray.Length > 0)
            {
                for (int i = 0; i < messageArray.Length; i++)
                {
                    var message = messageArray[i];
                    result += $"{i + 1}. Message Id: {message.MessageId}; " +
                    $"ExpiresOn: {message.ExpiresOn}; " +
                    $"InsertedOn: {message.InsertedOn}; " +
                    $"DequeueCount: {message.DequeueCount}; " +
                    $"Message: {message.Body}.";
                }
            }
            else
            {
                throw new RequestFailedException(404, "MessageId not found");
            }

            return result;
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> ReceiveMessageAsync()
    {
        try
        {
            var response = await _queueClient.ReceiveMessageAsync();
            var message = response.Value;
            if (message != null)
            {
                return $"Message Id: {message.MessageId}; " +
                $"ExpiresOn: {message.ExpiresOn}; " +
                $"InsertedOn: {message.InsertedOn}; " +
                $"DequeueCount: {message.DequeueCount}; " +
                $"Message: {message.Body}.";
            }
            else
            {
                throw new RequestFailedException(404, "MessageId not found");
            }
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> ReceiveMessagesAsync()
    {
        try
        {
            var response = await _queueClient.ReceiveMessagesAsync();
            var messageArray = response.Value;
            var result = string.Empty;
            if (messageArray.Length > 0)
            {
                for (int i = 0; i < messageArray.Length; i++)
                {
                    var message = messageArray[i];
                    result += $"{i + 1}. Message Id: {message.MessageId}; " +
                    $"ExpiresOn: {message.ExpiresOn}; " +
                    $"InsertedOn: {message.InsertedOn}; " +
                    $"DequeueCount: {message.DequeueCount}; " +
                    $"Message: {message.Body}.";
                }
            }
            else
            {
                throw new RequestFailedException(404, "MessageId not found");
            }

            return result;
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> SendMessageAsync(string messageText)
    {
        try
        {
            var response = await _queueClient.SendMessageAsync(messageText);
            var sendReceipt = response.Value;
            return $"Created queue message: {messageText}, with MessageId: {sendReceipt.MessageId} and popReceipt:{sendReceipt.PopReceipt}.";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }

    public async Task<string> UpdateMessageAsync(string messageId, string popReceipt, string message)
    {
        try
        {
            var response = await _queueClient.UpdateMessageAsync(messageId, popReceipt, message);
            var updateReceipt = response.Value;
            return $"Updated queue message: {message}; MessageId {messageId}; PopReceipt{updateReceipt.PopReceipt}";
        }
        catch (RequestFailedException e)
        {
            return GenerateError(e);
        }
    }
}