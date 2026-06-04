using Meloht.API.Gateway.BackendAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Meloht.API.Gateway.BackendAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginData request)
        {
            return Ok(new
            {
                request.Username,
                request.Password
            });
        }

        [HttpPost("save")]
        public IActionResult Save([FromForm] LoginData request)
        {
            return Ok(request);
        }
    }
}
