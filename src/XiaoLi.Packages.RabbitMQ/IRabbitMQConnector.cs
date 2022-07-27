using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace XiaoLi.Packages.RabbitMQ
{
    /// <summary>
    /// RabbitMQ--Channel工厂
    /// </summary>
    public interface IRabbitMQConnector:IDisposable
    {
        /// <summary>
        /// 连接还没断掉
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 保持活性
        /// </summary>
        void KeepAalive();

        /// <summary>
        /// 创建channel
        /// </summary>
        /// <returns></returns>
        IModel CreateChannel();
    }
}
