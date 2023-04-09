using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P230_Pronia.Entities;
using P230_Pronia.ViewModels;
using P230_Pronia.Utilities.Extensions;

namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInmanager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInmanager = signInManager;

        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return BadRequest();

            User user = new User()
            {
                UserName = register.Username,
                Fullname = string.Concat(register.Firstname, " ", register.Lastname),
                Email = register.Email

            };
            IdentityResult result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError massenger in result.Errors)
                {
                    ModelState.AddModelError("", massenger.Description);
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();

            User user = await _userManager.FindByNameAsync(login.Username);
            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInmanager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            if (!result.Succeeded)
            {
                if (!result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Due to overtyring your account has been blocked for 5 minutes");
                    return View();
                }
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInmanager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ShowAuthenticated()
        {
            return Json(User.Identity.IsAuthenticated);
        }
    }
}


