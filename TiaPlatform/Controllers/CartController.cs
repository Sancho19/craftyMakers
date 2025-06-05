using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaPlatform.Data;
using TiaPlatform.Models;

namespace TiaPlatform.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int resourceId)
        {
            var user = await _userManager.GetUserAsync(User);
            var resource = await _context.Resources.FindAsync(resourceId);

            if (resource == null)
                return NotFound();

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ResourceId == resourceId);

            if (existingItem != null)
            {
                return Json(new { success = false, message = "Item is already in your cart." });
            }

            var cartItem = new CartItem
            {
                ResourceId = resourceId,
                UserId = user.Id,
                Quantity = 1,
                AddedAt = DateTime.UtcNow
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item added to cart." });
        }



        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var items = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Resource)
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["CartMessage"] = "Item removed from cart!";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int change)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                item.Quantity = Math.Max(1, item.Quantity + change);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var userId = _userManager.GetUserId(User);
            var count = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            return Json(new { count });
        }


    }

}
