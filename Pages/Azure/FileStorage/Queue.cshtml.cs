using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AzureServices.Services.Storage.Queues;

namespace AzureServices.Pages.Azure.Queues;

[Authorize]
public class QueueModel : PageModel
{
    private readonly ILogger<QueueModel> _logger;

    private readonly IQueueService _queueService;

    private readonly string EmptyResponse = "Empty response";

    public QueueModel(ILogger<QueueModel> logger, IQueueService queueService)
    {
        _logger = logger;
        _queueService = queueService;
    }

    public string Message { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task OnPostPeekMessage()
    {
        Message = await _queueService.PeekMessageAsync();
    }

    public async Task OnPostPeekMessages()
    {
        Message = await _queueService.PeekMessagesAsync();
    }

    public async Task OnPostReceiveMessage()
    {
        Message = await _queueService.ReceiveMessageAsync();
    }

    public async Task OnPostReceiveMessages()
    {
        Message = await _queueService.ReceiveMessagesAsync();
    }

    public async Task OnPostSendMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Message = await _queueService.SendMessageAsync(message);
        }
        else
        {
            Message = EmptyResponse;
        }
    }

    public async Task OnPostClearQueue()
    {
        Message = await _queueService.ClearMessagesAsync();
    }

    public void OnPostReset()
    {
        Message = string.Empty;
    }

    public async Task OnPostDeleteMessage(string messageId)
    {
        if (!string.IsNullOrEmpty(messageId))
        {
            Message = await _queueService.DeleteMessageAsync(messageId);
        }
        else
        {
            Message = EmptyResponse;
        }
    }

    public async Task OnPostUpdateMessage(string message, string messageId, string popReceipt)
    {
        bool validInput = !string.IsNullOrEmpty(messageId) &&
            !string.IsNullOrEmpty(messageId) &&
            !string.IsNullOrEmpty(popReceipt);
        if (validInput)
        {
            Message = await _queueService.UpdateMessageAsync(messageId, popReceipt, message);
        }
        else
        {
            Message = EmptyResponse;
        }
    }
}