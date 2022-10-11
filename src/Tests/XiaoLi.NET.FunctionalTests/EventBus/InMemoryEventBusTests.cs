
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.EventBus;
using XiaoLi.NET.FunctionalTests.EventBus.Events;
using Xunit.Abstractions;

namespace XiaoLi.NET.FunctionalTests.EventBus;

public class InMemoryEventBusTests:EventBusScenarioBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public InMemoryEventBusTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void 发布一个本地事件()
    {
        var eventBus = ServiceProvider.GetRequiredService<IEventBus>();
        _testOutputHelper.WriteLine("游戏开始了");
        eventBus.Publish(new GameBeginEvent("LOL",10));
        
        // 阻塞主线程等待消费者处理完
        Thread.Sleep(4000);
    }
}