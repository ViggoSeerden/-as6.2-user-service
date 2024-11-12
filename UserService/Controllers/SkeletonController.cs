using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/skeleton")]
public class SkeletonController : ControllerBase
{
    private MessageProducer _sender = new();

    [HttpGet("")]
    public string GetSkeletonMessage()
    {
        return "This is the User Service Skeleton endpoint.";
    }    
    
    [HttpGet("skeleton/email")]
    public string? SendSkeletonEmail()
    {
        _sender.SendMessage();
        var response = MessageReceiver.GetConsumedMessage();
        return response;
    }
}