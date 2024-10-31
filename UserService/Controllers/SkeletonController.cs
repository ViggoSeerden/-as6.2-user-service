using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
[Route("users")]
public class SkeletonController : ControllerBase
{
    [HttpGet("skeleton")]
    public string GetSkeletonMessage()
    {
        return "This is the User Service Skeleton endpoint.";
    }
}