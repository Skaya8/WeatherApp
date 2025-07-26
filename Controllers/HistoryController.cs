using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using WeatherApp.Services.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly ICityRepository _cityRepository;
        
        public HistoryController(IWeatherService weatherService, ICityRepository cityRepository)
        {
            _weatherService = weatherService;
            _cityRepository = cityRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? city = null, string? condition = null, int page = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            int pageSize = 10;
            var (results, totalCount) = await _weatherService.GetWeatherSearchesPagedAsync(
                userId, city, condition, null, null, null, page, pageSize);

            ViewBag.Cities = await _cityRepository.GetAllCitiesAsync();
            ViewBag.City = string.IsNullOrEmpty(city) ? "" : city;
            ViewBag.Condition = string.IsNullOrEmpty(condition) ? "" : condition;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return View(results);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges([FromBody] List<WeatherApp.Models.WeatherSearchResult> changes)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            foreach (var change in changes)
            {
                await _weatherService.UpdateWeatherSearchAsync(change, userId.Value);
            }

            return Json(new { message = "Changes saved successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetChangeLog(int id)
        {
            var logs = await _weatherService.GetChangeLogAsync(id);
            return Json(logs);
        }
    }
} 