using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AutoConfig;

[Route("[controller]/[action]")]
[ApiController]
public class TestController:ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IOptions<TestValidateOptions> _options;

    public TestController(ILogger<TestController> logger, IOptions<TestValidateOptions> options)
    {
        _logger = logger;
        _options = options;
    }


    [HttpGet]
    public string GetValidateOptions()
    {
        return _options.Value.A;
    }
}