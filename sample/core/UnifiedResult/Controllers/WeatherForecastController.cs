using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using XiaoLi.NET.UnifiedResult;
using XiaoLi.NET.UnifiedResult.Attributes;

namespace UnifiedResult.Controllers;

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

    /// <summary>
    /// 带SuppressUnifiedResult的不会被unified filter handle
    /// </summary>
    /// <returns></returns>
    [SuppressUnifiedResult]
    [HttpGet]
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
    [HttpGet]
    public IEnumerable<WeatherForecast> Get2()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    [HttpGet]
    public int Get3()
    {
        return 1;
    }
    
    [HttpGet]
    public void Get4()
    {
        Console.WriteLine(123);
    }

    [HttpPost]
    public DemoPlayload Post(DemoPlayload playload)
    {
        throw new Exception();
        return playload;
    }
    
    [HttpPost]
    public UnifiedResponse Post2(DemoPlayload playload)
    {
        var res = new UnifiedResponse();

        res.Data = playload;
        return res;
    }
    
    
    public class DemoPlayload
    {
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}