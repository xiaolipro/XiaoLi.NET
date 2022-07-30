using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace XiaoLi.NET.RabbitMQ
{
    /// <summary>
    /// RabbitMQ--Channel工厂
    /// </summary>
    public interface IRabbitMQConnector:IDisposable
    {
        /// <summary>
        /// 保持活性
        /// </summary>
        void KeepAlive();

        /// <summary>
        /// 创建channel
        /// </summary>
        /// <returns></returns>
        IModel CreateChannel();
    }
}
