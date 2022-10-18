
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.EventBus;
using XiaoLi.NET.FunctionalTests.EventBus.EventHandlers;
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
        var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        var eventBus = ServiceProvider.GetRequiredService<IEventBus>();
        _testOutputHelper.WriteLine("进入游戏");
        eventBus.Publish(new GameBeginEvent("LOL",10));

        var timer = new System.Timers.Timer(500);
        timer.Elapsed += (sender, args) =>
        {
            _testOutputHelper.WriteLine(GameBeginEventHandler.Message);
            if (GameBeginEventHandler.Message.Equals("初始化完毕！")) waitHandle.Set();
            throw new Exception("G");
        };
        timer.Start();

        waitHandle.WaitOne();
        _testOutputHelper.WriteLine("游戏开始");
    }
}