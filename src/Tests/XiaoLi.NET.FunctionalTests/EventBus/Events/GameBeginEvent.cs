using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.FunctionalTests.EventBus.Events;

public class GameBeginEvent:IntegrationEvent
{
    public GameBeginEvent(string gameType, int peopleNumber)
    {
        GameType = gameType;
        PeopleNumber = peopleNumber;
    }

    public string GameType { get; set; }
    public int PeopleNumber { get; set; }
}