using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace XiaoLi.RabbitMQ
{
    /// <summary>
    /// RabbitMQ连接器
    /// </summary>
    public interface IRabbitMQConnector:IDisposable
    {
        /// <summary>
        /// 连接是否已关闭
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建channel
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
