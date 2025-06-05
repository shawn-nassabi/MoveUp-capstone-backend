using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("error")]
    public IActionResult HandleError() =>
        Problem(); // Returns standardized RFC 7807 response
}
// This api deals with unhandled errors, and returns clean error responses to the client