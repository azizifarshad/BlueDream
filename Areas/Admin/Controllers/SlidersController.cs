using Microsoft.AspNetCore.Mvc;
using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Hosting;

namespace BlueDream.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlidersController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Sliders
        public IActionResult Index()
        {
            var sliders = _context.Sliders.ToList();
            return View(sliders);
        }

        // GET: Admin/Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        [HttpPost]
        public IActionResult Create(Slider model, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads/sliders");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                imageFile.CopyTo(stream);

                model.ImageUrl = "/uploads/sliders/" + fileName;
            }

            _context.Sliders.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Admin/Sliders/Edit/5
        public IActionResult Edit(int id)
        {
            var slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Slider model, IFormFile imageFile)
        {
            var slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();

            slider.Title = model.Title;
            slider.Description = model.Description;

            if (imageFile != null && imageFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads/sliders");
                Directory.CreateDirectory(folder);

                // حذف تصویر قبلی
                if (!string.IsNullOrEmpty(slider.ImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                imageFile.CopyTo(stream);

                slider.ImageUrl = "/uploads/sliders/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();

            if (!string.IsNullOrEmpty(slider.ImageUrl))
            {
                var path = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
