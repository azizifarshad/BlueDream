using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueDream.Data;
using BlueDream.Models.ViewModels;

namespace BlueDream.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /About
        public async Task<IActionResult> Index()
        {
            var socialLinks = await _context.SocialLinks
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToListAsync();

            var vm = new AboutViewModel
            {
                BusinessName = "Blue Dreams Curly Hair",
                Address = "Cas Oorthuyskade 330, Amsterdam",
                IntroHtml = @"<p>Welcome to <strong>Blue Dreams by Sahra</strong> — a place where creativity meets precision.
                I specialise in colour transformations, highlights, lowlights, and personalised treatments that bring out the best in your hair.</p>
                <p>A wet haircut on curly hair gives you the chance to see how beautiful the style looks even when it’s straight.
                Whether you’re after a subtle refresh or a complete new look, I’ll make sure you leave feeling confident and beautiful.</p>",
                Amenities = "Amenities",
                PaymentMethods = new System.Collections.Generic.List<string>
                {
                    "Cash accepted",
                    "Credit card accepted",
                    "Debit card accepted"
                },
                Languages = new System.Collections.Generic.List<string>
                {
                    "Dutch"
                },
                ProductsUsed = "Schwarzkopf Professional",
                SocialLinks = socialLinks
            };

            return View(vm);
        }
    }
}