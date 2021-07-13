using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raymaker.FeatureManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFeatureManager _featureManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        // GET weatherforecast/
        [HttpGet]
        [FeatureGate(MyFeatureFlags.FeatureA)]
        public async Task<IEnumerable<WeatherForecast>> GetCities()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // GET weatherforecast/getByCity/CPH
        [HttpGet("GetByCity/{name}")]
        public async Task<WeatherForecast> GetByCity(string name)
        {
            if (await _featureManager.IsEnabledAsync("FeatureC"))
            {
                return new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = 5,
                    Summary = "Chilly winters day in " + name
                };
            }

            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 26,
                Summary = "Hot summers day in " + name
            };
        }

        // GET weatherforecast/getBySeason/summer
        [HttpGet("GetBySeason/{season}")]
        [FeatureGate(MyFeatureFlags.FeatureHoliday)]
        public WeatherForecast GetBySeason(string season)
        {
            var rnd = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = rnd.Next(-5,30),
                Summary = "Happy holidays in " + season + ". " + DateTime.UtcNow.ToString("O")
            };
        }
    }
}
