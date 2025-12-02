using BlueDream.Data;
using BlueDream.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace BlueDream.Areas.Admin.Controllers
{

    public class SocialLinksController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public SocialLinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SocialLinks
        public IActionResult Index()
        {
            var links = _context.SocialLinks.ToList();
            return View(links);
        }

        // GET: Admin/SocialLinks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SocialLinks/Create
        [HttpPost]
        public IActionResult Create(SocialLink model, IFormFile iconFile)
        {
            if (iconFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/icons");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(iconFile.FileName);
                var full = Path.Combine(folder, fileName);

                using (var stream = new FileStream(full, FileMode.Create))
                {
                    iconFile.CopyTo(stream);
                }

                model.IconUrl = "/images/icons/" + fileName;
            }

            _context.SocialLinks.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/SocialLinks/Edit/5
        public IActionResult Edit(int id)
        {
            var link = _context.SocialLinks.Find(id);
            if (link == null) return NotFound();
            return View(link);
        }

        // POST: Admin/SocialLinks/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, SocialLink model, IFormFile iconFile)
        {
            var link = _context.SocialLinks.Find(id);
            if (link == null) return NotFound();

            link.Title = model.Title;
            link.Url = model.Url;

            if (iconFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/icons");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // حذف آیکون قبلی
                if (!string.IsNullOrEmpty(link.IconUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", link.IconUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(iconFile.FileName);
                var full = Path.Combine(folder, fileName);
                using (var stream = new FileStream(full, FileMode.Create))
                {
                    iconFile.CopyTo(stream);
                }
                link.IconUrl = "/images/icons/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Admin/SocialLinks/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var link = _context.SocialLinks.Find(id);
            if (link == null) return NotFound();

            // حذف آیکون از فولدر
            if (!string.IsNullOrEmpty(link.IconUrl))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", link.IconUrl.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.SocialLinks.Remove(link);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
