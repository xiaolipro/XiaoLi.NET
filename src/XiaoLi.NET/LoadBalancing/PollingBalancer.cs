
using System.Collections.Generic;

namespace XiaoLi.NET.LoadBalancing
{
    /// <summary>
    /// 轮询调度器
    /// </summary>
    public class PollingBalancer : IBalancer
    {
        private static int _counter;
        public string Name { get; } = nameof(PollingBalancer);
        public int Pick(List<dynamic> services)
        {
            lock (this)
            {
                if (_counter > 0x3fffffff) _counter = 0;
                return _counter ++ % services.Count;
            }
        }
    }
}
