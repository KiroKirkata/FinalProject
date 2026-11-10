using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Condominium.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() { Response.StatusCode = 500; return View(); }
}
