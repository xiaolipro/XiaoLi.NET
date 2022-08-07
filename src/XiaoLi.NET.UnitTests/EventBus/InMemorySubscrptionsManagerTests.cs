
namespace XiaoLi.NET.App.UnitTests.EventBus
{
    public class InMemorySubscrptionsManagerTests
    {
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var manager = new InMemorySubscriptionsManager();

            Assert.True(manager.IsEmpty);
        }

        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemorySubscriptionsManager();
            manager.AddSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            Assert.True(manager.HasSubscriptions<NumberChangeEvent>());
        }

        [Fact]
        public void After_One_Dynamic_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemorySubscriptionsManager();
            manager.AddDynamicSubscription<NumberChangeDynamicEventHandler>("NumberChangeEvent");
            Assert.True(manager.HasSubscriptions<NumberChangeEvent>());
        }

        [Fact]
        public void After_All_Subscriptions_Are_Deleted_Event_Should_No_Longer_Exists()
        {
            var manager = new InMemorySubscriptionsManager();
            manager.AddSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            manager.AddDynamicSubscription<NumberChangeDynamicEventHandler>("NumberChangeEvent");
            manager.RemoveSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            Assert.True(manager.HasSubscriptions<NumberChangeEvent>());
            manager.RemoveDynamicSubscription<NumberChangeDynamicEventHandler>("NumberChangeEvent");
            Assert.False(manager.HasSubscriptions<NumberChangeEvent>());
        }


        [Fact]
        public void Deleting_Last_Subscription_Should_Raise_On_Deleted_Event()
        {
            // arrange
            bool raised = false;
            var manager = new InMemorySubscriptionsManager();
            manager.OnEventRemoved += (o, e) => raised = true;

            // act
            manager.AddSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            manager.RemoveSubscription<NumberChangeEvent, NumberChangeEventHandler>();

            // assert
            Assert.True(raised);
        }

        [Fact]
        public void Clear_Subscription_Should_No_Raise_On_Deleted_Event()
        {
            // arrange
            bool raised = false;
            var manager = new InMemorySubscriptionsManager();
            manager.OnEventRemoved += (o, e) => raised = true;

            // act
            manager.AddSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            manager.Clear();

            // assert
            Assert.False(raised);
        }

        [Fact]
        public void Get_Handlers_For_Event_Should_Return_All_Handlers()
        {
            var manager = new InMemorySubscriptionsManager();
            manager.AddSubscription<NumberChangeEvent, NumberChangeEventHandler>();
            manager.AddDynamicSubscription<NumberChangeDynamicEventHandler>("NumberChangeEvent");
            var infos = manager.GetSubscriptionInfos<NumberChangeEvent>();
            Assert.Equal(2, infos.Count());
        }
    }
}
