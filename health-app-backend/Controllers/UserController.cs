using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}