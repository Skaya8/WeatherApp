using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IValidationService _validationService;
        public AccountController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = await _validationService.ValidateUserCredentialsAsync(model.Username ?? "", model.Password ?? "");
            
            if (userId != null)
            {
                HttpContext.Session.SetInt32("UserId", userId.Value);
                HttpContext.Session.SetString("Username", model.Username ?? "");
                return RedirectToAction("Index", "Weather");
            }
            model.ErrorMessage = "Invalid username or password";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
} 