using System.Security.Claims;
using Condominium.Data;
using Condominium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Condominium.Web.Controllers;

[Authorize(Roles = Roles.Resident + "," + Roles.Manager + "," + Roles.Administrator)]
public abstract class OverviewController : Controller
{
    protected UserAccess Access => new(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Липсва идентификатор на потребителя."),
        User.IsInRole(Roles.Administrator), User.IsInRole(Roles.Manager));
}
