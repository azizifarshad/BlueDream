using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BlueDream.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController 

    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["ActivePage"] = "Dashboard";
            return View();
        }
    }
}