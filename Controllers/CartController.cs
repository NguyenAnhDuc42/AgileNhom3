using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Agile3.Data;
using Agile3.Models;

namespace Agile3.Controllers
{
    public class CartController : Controller
    {
        private readonly dataContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(dataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Auth");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Auth");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = 1,
                    Total = product.Price
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
                cartItem.Total = product.Price * cartItem.Quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null) return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(decimal amountPaid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Auth");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction(nameof(Index));

            decimal total = cartItems.Sum(c => c.Total);

            var order = new Order
            {
                UserId = user.Id,
                CustomerName = user.UserName ?? "Khách",
                PhoneNumber = user.PhoneNumber ?? "",
                TotalAmount = total,
                AmountPaid = amountPaid,
                ChangeGiven = amountPaid - total,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Total / item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Order", new { id = order.Id });
        }

    }
}