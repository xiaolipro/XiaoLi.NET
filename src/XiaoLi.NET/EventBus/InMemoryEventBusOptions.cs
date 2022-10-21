#if NETCOREAPP
namespace XiaoLi.NET.EventBus
{
    public class InMemoryEventBusOptions
    {
        /// <summary>
        /// 事件总线容量
        /// </summary>
        public int Capacity { get; set; }
    }
}
#endif