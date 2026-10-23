
using Damgio.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damgio.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController : Controller
{
    // GET: /Admin
    public IActionResult Index()
    {
        return View();
    }
}
