using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace XiaoLi.NET.RabbitMQ.Connect
{
    /// <summary>
    /// RabbitMQ--Channel工厂
    /// </summary>
    public interface IRabbitMQConnector : IDisposable
    {
        /// <summary>
        /// 保持连接活性
        /// </summary>
        void KeepAlive();

        /// <summary>
        /// 创建channel
        /// </summary>
        /// <returns></returns>
        IModel CreateChannel();
    }
}
