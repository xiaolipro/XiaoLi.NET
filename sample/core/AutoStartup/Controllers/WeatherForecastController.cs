using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AutoStartup.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }


    [HttpPost]
    public Output GetDt(Input input)
    {
        return new Output()
        {
            time = input.time,
            time2 = input.time2
        };
    }
    public class Output
    {
        public DateTime? time { get; set; }
        public DateTime time2 { get; set; }
    }
    public class Input
    {
        public string Name { get; set; }
        public DateTime? time { get; set; }
        public DateTime time2 { get; set; }
    }
}