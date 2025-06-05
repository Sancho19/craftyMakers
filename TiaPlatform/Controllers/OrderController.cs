using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaPlatform.Data;
using TiaPlatform.Models;

namespace TiaPlatform.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> MyPurchases()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Resource)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

    }
}
