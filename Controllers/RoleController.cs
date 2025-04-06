using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Agile3.Models;

namespace Agile3.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Initialize()
        {
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AdminAdd()
        {
            var user = await _userManager.FindByEmailAsync("admin@gmail.com");
            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("Index", "Home");
        }
    }
}