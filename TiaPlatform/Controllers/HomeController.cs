using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaPlatform.Data;
using TiaPlatform.Models;

namespace TiaPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var resources = await _context.Resources
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(resources);
        }
        public async Task<IActionResult> GetFilteredResources(string search)
        {
            var resources = await _context.Resources
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            if (!string.IsNullOrEmpty(search))
                resources = resources.Where(r => r.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            return PartialView("ResourceGrid", resources);
        }


        public async Task<IActionResult> Details(int id)
        {
            var resource = await _context.Resources
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null) return NotFound();

            return View(resource);
        }

    }
}
