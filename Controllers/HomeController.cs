using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Agile3.Models;
using Agile3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Agile3.Controllers;

public class HomeController(dataContext context) : Controller
{

    public async Task<IActionResult> Index()
    {
        var products = await context.Products
            .Include(p => p.Category)
            .ToListAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(context.Categories, "Id", "Name");
        return View(new Product());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price,Quantity,CategoryId")] Product product)
    {
        if (ModelState.IsValid)
        {
            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
            }
        }
        // Repopulate categories if validation fails
        ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            context.Update(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    private bool ProductExists(int id)
    {
        return context.Products.Any(e => e.Id == id);
    }
}
