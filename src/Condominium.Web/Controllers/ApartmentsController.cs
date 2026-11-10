using Condominium.Services;
using Microsoft.AspNetCore.Mvc;
namespace Condominium.Web.Controllers;
public class ApartmentsController(IOverviewService service) : OverviewController
{
    public async Task<IActionResult> Index() => View(await service.ApartmentsAsync(Access));
}
