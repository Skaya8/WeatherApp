using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            return View(new WeatherViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] WeatherApp.Models.WeatherViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            await _weatherService.SaveWeatherSearchAsync(model, userId.Value);

            return Json(new { message = "Weather data saved successfully!" });
        }
    }
} 