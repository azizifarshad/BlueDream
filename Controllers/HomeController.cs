using System.Diagnostics;
using BlueDream.Data;
using BlueDream.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlueDream.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // پاس دادن لینک‌های شبکه‌های اجتماعی به View
            ViewBag.SocialLinks = _context.SocialLinks.ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}