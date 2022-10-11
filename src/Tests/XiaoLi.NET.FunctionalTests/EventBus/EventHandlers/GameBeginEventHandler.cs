using Microsoft.Extensions.Logging;
using XiaoLi.NET.EventBus.Events;
using XiaoLi.NET.FunctionalTests.EventBus.Events;

namespace XiaoLi.NET.FunctionalTests.EventBus.EventHandlers;

public class GameBeginEventHandler:IIntegrationEventHandler<GameBeginEvent>
{
    private readonly ILogger<GameBeginEventHandler> _logger;

    public GameBeginEventHandler(ILogger<GameBeginEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(GameBeginEvent @event)
    {
        _logger.LogInformation("初始化游戏数据...");
        // business...
        await Task.Delay(3000);
        _logger.LogInformation("初始化完毕，游戏开始！");
    }
}