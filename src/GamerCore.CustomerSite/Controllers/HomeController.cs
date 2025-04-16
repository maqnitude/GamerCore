using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // public async Task<IActionResult> Index()
        // {
        //     try
        //     {
        //         var client = _httpClientFactory.CreateClient("GamerCoreDev");
        //         var response = await client.GetAsync("/api/weatherforecast");
        //
        //         if (response.IsSuccessStatusCode)
        //         {
        //             using var contentStream = await response.Content.ReadAsStreamAsync();
        //             var weatherForecasts = await JsonSerializer
        //                 .DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
        //
        //             return View(weatherForecasts);
        //         }
        //         else
        //         {
        //             _logger.LogWarning($"Request failed with status code: {response.StatusCode}");
        //
        //             return RedirectToAction("Error");
        //         }
        //     }
        //     catch (HttpRequestException exception)
        //     {
        //         _logger.LogError(exception, "An error occured while processing the request");
        //
        //         return RedirectToAction("Error");
        //     }
        // }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}