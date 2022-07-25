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
    public interface IChannelFactory:IDisposable
    {
        /// <summary>
        /// 是否连接上
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 创建channel
        /// </summary>
        /// <returns></returns>
        IModel CreateChannel();
    }
}
