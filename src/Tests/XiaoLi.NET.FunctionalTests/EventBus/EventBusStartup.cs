using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.EventBus;
using XiaoLi.NET.EventBus.Subscriptions;
using XiaoLi.NET.FunctionalTests.EventBus.EventHandlers;
using XiaoLi.NET.FunctionalTests.EventBus.Events;

namespace XiaoLi.NET.FunctionalTests.EventBus;

public class EventBusStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISubscriptionsManager, InMemorySubscriptionsManager>();
        services.Configure<InMemoryEventBusOptions>(x => x.Capacity = 10);
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        services.AddTransient<GameBeginEventHandler>();
    }

    public void Configure(IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<GameBeginEvent,GameBeginEventHandler>();
    }
}