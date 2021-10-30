using System.Threading.Tasks;
using Core_3._1.Entities;
using Core_3._1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLE_System.Models;

namespace Core_3._1.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                       new { userId = user.Id, code = code }, protocol: Request.Scheme);
                    await _emailService.SendEmailAsync(model.Email, "Confirm your account",
                        "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("EmailConfirmingNotice", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View("EmailConfirmed");
            }

            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmail()
        {
            return View("ResendEmailForm");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmail(ResendEmailViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.EmailConfirmed == true)
            {
                ModelState.AddModelError(string.Empty, "Email not exist or has been confirmed!");

                return View("ResendEmailForm", model);
            }

            await SendMail(user, model.Email);

            return RedirectToAction("EmailConfirmingNotice", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailConfirmingNotice()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                var identityUser = await _userManager.FindByEmailAsync(user.Email);
                if (identityUser != null)
                {
                    if (!identityUser.EmailConfirmed)
                    {
                        ModelState.AddModelError(string.Empty, "Please confirm email!");

                        return View(user);
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        private async Task SendMail(ApplicationUser user, string toEmail)
        {
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = user.Id, code = code }, protocol: Request.Scheme);
            await _emailService.SendEmailAsync(toEmail, "Confirm your account",
                "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }
    }
}
