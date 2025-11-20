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
            // لینک شبکه‌های اجتماعی
            ViewBag.SocialLinks = _context.SocialLinks.ToList();

            // لیست اسلایدرها
            ViewBag.Sliders = _context.Sliders.ToList();

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