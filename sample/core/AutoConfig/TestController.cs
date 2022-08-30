using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AutoConfig;

[Route("[controller]/[action]")]
[ApiController]
public class TestController:ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IOptionsMonitor<TestValidateOptions> _options;

    public TestController(ILogger<TestController> logger, IOptionsMonitor<TestValidateOptions> options)
    {
        _logger = logger;
        _options = options;
    }


    [HttpGet]
    public string GetValidateOptions()
    {
        return _options.CurrentValue.A;
    }
}