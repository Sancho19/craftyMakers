using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using TiaPlatform.Data;
using System.Data;

using TiaPlatform.Models;

namespace TiaPlatform.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Public view of all resources
        public async Task<IActionResult> Index()
        {
            var resources = await _context.Resources.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return View(resources);
        }

        // View details
        public async Task<IActionResult> Details(int id)
        {
            var resource = await _context.Resources.FirstOrDefaultAsync(r => r.Id == id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        // Admin-only: Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Resource model, IFormFile file, List<IFormFile> previewImages)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.FilePath = "/uploads/" + fileName;
            }

            model.CreatedAt = DateTime.UtcNow;
            model.Images = new List<ResourceImage>();

            if (previewImages != null && previewImages.Any())
            {
                foreach (var img in previewImages)
                {
                    if (img.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/previews", imgName);

                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        model.Images.Add(new ResourceImage
                        {
                            ImagePath = "/uploads/previews/" + imgName
                        });
                    }
                }
            }

            _context.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminDashboard));
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var resources = await _context.Resources
    .Include(r => r.Images)
    .OrderByDescending(r => r.CreatedAt)
    .ToListAsync();

            return View(resources);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsModal(int id)
        {
            var resource = await _context.Resources
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                return NotFound();

            return PartialView("ResourceDetailsModal", resource);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Resource model, IFormFile file, List<IFormFile> newPreviewImages)
        {
            var resource = await _context.Resources
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (resource == null) return NotFound();

            resource.Title = model.Title;
            resource.Subject = model.Subject;
            resource.Description = model.Description;
            resource.Price = model.Price;
            resource.LastUpdatedAt = DateTime.UtcNow;

            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                resource.FilePath = "/uploads/" + fileName;
            }

            if (newPreviewImages != null && newPreviewImages.Any())
            {
                foreach (var img in newPreviewImages)
                {
                    if (img.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/previews", imgName);

                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        resource.Images.Add(new ResourceImage
                        {
                            ImagePath = "/uploads/previews/" + imgName,
                            ResourceId = resource.Id
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(); // for AJAX
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FilterDashboard(string subject, string fromDate, string toDate)
        {
            var query = _context.Resources.Include(r => r.Images).AsQueryable();

            if (!string.IsNullOrEmpty(subject))
                query = query.Where(r => r.Subject == subject);

            if (DateTime.TryParse(fromDate, out var fromDt))
                query = query.Where(r => r.CreatedAt >= fromDt);

            if (DateTime.TryParse(toDate, out var toDt))
                query = query.Where(r => r.CreatedAt <= toDt);

            var filtered = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            return PartialView("ResourceCards", filtered);
        }




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminDashboard));
        }

    }
}
