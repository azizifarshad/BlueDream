using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueDream.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BaseAdminController : Controller
    {
        protected void SetPageData(string title, string activePage)
        {
            ViewData["Title"] = title;
            ViewData["ActivePage"] = activePage;
        }

        protected IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;
            return View("/Views/Shared/NotFound.cshtml");
        }

        protected IActionResult AccessDeniedPage()
        {
            Response.StatusCode = 403;
            return View("/Views/Shared/AccessDenied.cshtml");
        }
    }
}