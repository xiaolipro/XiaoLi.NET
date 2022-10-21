using Microsoft.Extensions.Logging;
using XiaoLi.NET.EventBus.Events;
using XiaoLi.NET.FunctionalTests.EventBus.Events;
using Xunit.Abstractions;

namespace XiaoLi.NET.FunctionalTests.EventBus.EventHandlers;

public class GameBeginEventHandler:IEventHandler<GameBeginEvent>
{
    private readonly ILogger<GameBeginEventHandler> _logger;
    public static string Message = string.Empty;

    public GameBeginEventHandler(ILogger<GameBeginEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(GameBeginEvent @event)
    {
        Message="初始化游戏数据...";
        // business...
        await Task.Delay(3000);
        Message="初始化完毕！";
    }
}