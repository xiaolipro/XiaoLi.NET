using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Extensions;

namespace XiaoLi.NET.FunctionalTests.DependencyInjection;

public class DependencyInjectionScenarioBase:ScenarioBase<DependencyInjectionStartup>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        return base.CreateHostBuilder().InitApp();
    }
}