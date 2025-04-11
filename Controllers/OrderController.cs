using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agile3.Models;
using Agile3.Data;
using Microsoft.AspNetCore.Identity;

public class OrderController : Controller
{
    private readonly dataContext _context;
    private readonly UserManager<User> _userManager; // ✅ Thêm dòng này

    public OrderController(dataContext context, UserManager<User> userManager) // ✅ Thêm tham số
    {
        _context = context;
        _userManager = userManager; // ✅ Gán vào field
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.Include(o => o.Items).ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(); // hoặc redirect về Login
        }

        order.UserId = user.Id;
        order.OrderDate = DateTime.Now;
        order.ChangeGiven = order.AmountPaid - order.TotalAmount;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Order updated)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.CustomerName = updated.CustomerName;
        order.PhoneNumber = updated.PhoneNumber;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
