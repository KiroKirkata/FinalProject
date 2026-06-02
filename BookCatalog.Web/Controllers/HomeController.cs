using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [ResponseCache(Duration = 0, NoStore = true)]
    public IActionResult Error() => View();

    // Custom page for 400, 401, 404, etc. The {code} comes from
    // UseStatusCodePagesWithReExecute in Program.cs.
    public IActionResult StatusCode(int code)
    {
        ViewBag.Code = code;
        ViewBag.Message = code switch
        {
            400 => "Bad Request — the server couldn't understand your request.",
            401 => "Unauthorized — you need to sign in to view this.",
            403 => "Forbidden — you don't have permission for this.",
            404 => "Not Found — that page doesn't exist.",
            _ => "Something went wrong."
        };
        return View();
    }
}
