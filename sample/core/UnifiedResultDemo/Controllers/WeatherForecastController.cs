using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using XiaoLi.NET.Mvc;
using XiaoLi.NET.Mvc.Exceptions;
using XiaoLi.NET.Mvc.UnifiedResults;

namespace UnifiedResultDemo.Controllers;

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
    // [SuppressUnifiedResult]
    [HttpGet]
    public (IEnumerable<WeatherForecast>,int,int) Get()
    {
        return (Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray(),10,200);
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
    public string Get4()
    {
        return "123333ste";
    }
    
    [HttpGet]
    public Task Get5()
    {
        Console.WriteLine(123);

        return Task.CompletedTask;
    }
    
    [HttpGet]
    public BaseResult Get6()
    {
        var res = UnifiedResultFactory.CreateBaseResult();
        Console.WriteLine(123);

        return res;
    }

    [HttpPost]
    public DemoPlayload Post(DemoPlayload playload)
    {
        throw new Exception();
        return playload;
    }
    
    [HttpPost]
    public DemoPlayload Post2(DemoPlayload playload)
    {
        return playload;
    }
    
    [HttpPost]
    public ActionResult Post3(DemoPlayload playload)
    {
        var res = UnifiedResultFactory.CreateDataResult<DemoPlayload>();
        res.Data = playload;
        return Ok(res);
    }
    
    [HttpPost]
    public DataResult<DemoPlayload> Post4(DemoPlayload playload)
    {
        var res = UnifiedResultFactory.CreateDataResult<DemoPlayload>();
        res.Data = playload;
        return res;
    }
    
    [HttpPost]
    public ActionResult Post5(DemoPlayload playload)
    {
        throw new BussinessException("GG");
    }

    public class DemoPlayload
    {
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}