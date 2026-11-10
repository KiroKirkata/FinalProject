using Condominium.Services;
using Microsoft.AspNetCore.Mvc;
namespace Condominium.Web.Controllers;
public class VotingsController(IOverviewService service) : OverviewController
{
    public async Task<IActionResult> Index() => View(await service.VotingsAsync(Access));
}
