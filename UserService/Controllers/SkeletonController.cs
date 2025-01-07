using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/skeleton")]
public class SkeletonController : ControllerBase
{
    private readonly MessageProducer _sender;

    public SkeletonController(MessageProducer sender)
    {
        _sender = sender;
    }

    [HttpGet("")]
    public string GetSkeletonMessage()
    {
        return "This is the User Service Skeleton endpoint.";
    }    
    
    [HttpPost("email")]
    public async Task<string?> SendSkeletonEmail(string email)
    {
        var requestId = Guid.NewGuid();
        var emailBytes = System.Text.Encoding.UTF8.GetBytes(email);
        _sender.SendMessage($"email.request.{requestId}.{Convert.ToBase64String(emailBytes)}", "UserEmail");

        var timeout = TimeSpan.FromSeconds(5); 
        var cancellationTokenSource = new CancellationTokenSource(timeout);
        var cancellationToken = cancellationTokenSource.Token;

        // Try to get the consumed message within the timeout period
        while (!cancellationToken.IsCancellationRequested)
        {
            var response = MessageReceiver.GetConsumedMessage(requestId.ToString());
            if (response != null)
            {
                return response;
            }
            
            await Task.Delay(100, cancellationToken);
        }
        
        return "The response took too long to arrive.";
    }
}