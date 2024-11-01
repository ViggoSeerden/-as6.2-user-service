using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Services;

namespace UserService.Controllers;

[ApiController]
[Route("users")]
public class SkeletonController : ControllerBase
{
    private MessageProducer _sender = new();

    [HttpGet("skeleton")]
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