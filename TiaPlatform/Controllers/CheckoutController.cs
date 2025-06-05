using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaPlatform.Data;
using TiaPlatform.Models;

namespace TiaPlatform.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Resource)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var total = cartItems.Sum(i => i.Resource.Price);

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                TotalPrice = total,
                Items = cartItems.Select(c => new OrderItem
                {
                    ResourceId = c.ResourceId,
                    Price = c.Resource.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems); // Clear cart
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
