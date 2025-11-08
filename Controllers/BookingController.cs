using BlueDream.Data;
using BlueDream.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlueDream.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ItemGroups)
                .ThenInclude(g => g.Items)
                .ToListAsync();

            var vm = new BookingViewModel
            {
                Categories = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c is { } ? c.Name + " services" : "",
                    ItemGroups = c.ItemGroups.Select(g => new ItemGroupVM
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        Items = g.Items.Select(i => new ItemVM
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            Price = i.Price,
                            Discount = i.Discount,
                            TimeSpend = i.TimeSpend
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return View(vm);
        }
    }
}