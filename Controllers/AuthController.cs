using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Agile3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Agile3.Controllers
{
    public class AuthController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // First find user by email to get their username
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                // Use the username for sign in
                var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Add error message for failed login
                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add user to Customer role
                    await userManager.AddToRoleAsync(user, "Customer");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.UserName = updatedUser.UserName;
            user.PhoneNumber = updatedUser.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // ✅ Đăng nhập lại để cập nhật User.Identity
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(updatedUser);
        }

        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            return View(user);
        }


    }
}
