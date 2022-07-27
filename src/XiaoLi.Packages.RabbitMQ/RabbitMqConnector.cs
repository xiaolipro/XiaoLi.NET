using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using XiaoLi.Packages.RabbitMQ.Options;

namespace XiaoLi.Packages.RabbitMQ
{
    public class RabbitMqConnector : IRabbitMQConnector
    {
        private readonly ILogger<RabbitMqConnector> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _retries;
        private IConnection _connection;
        private bool _disposed;
        private readonly object _lock = new object();


        public RabbitMqConnector(ILogger<RabbitMqConnector> logger, IOptions<RabbitMQClientOptions> config, int retries = 5)
        {
            _logger = logger;
            var clientConfig = config.Value;
            _connectionFactory = GetConnectionFactory(clientConfig);
            _retries = retries;
        }


        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;


        public IModel CreateChannel()
        {
            if (!IsConnected)
            {
                ReConnect();
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.ConnectionBlocked -= OnConnectionBlocked;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public void KeepAalive()
        {
            if(!IsConnected) ReConnect();
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <exception cref="Exception">RabbitMQ connections could not be created and opened</exception>
        private void ReConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_lock)
            {
                var policy = Policy.Handle<SocketException>() //socket异常时
                    .Or<BrokerUnreachableException>() //broker不可达异常时
                    .WaitAndRetry(_retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            _logger.LogWarning(ex,
                                "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                                $"{time.TotalSeconds:n1}", ex.Message);
                        }
                    );

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (!IsConnected) throw new Exception("FATAL ERROR: RabbitMQ connections could not be created and opened");

                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;

                // RabbitMQ出于自身保护策略，通过阻塞方式限制写入，导致了生产者应用“假死”，不再对外服务。
                // 比若说CPU/IO/RAM资源下降，队列堆积，导致堵塞，就会触发这个事件
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation(
                    "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                    _connection.Endpoint.HostName);
            }

        }

        /// <summary>
        /// 获取连接工厂
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IConnectionFactory GetConnectionFactory(RabbitMQClientOptions config)
        {
            var connectionFactory = new ConnectionFactory()
            {
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrWhiteSpace(config.HostName))
            {
                connectionFactory.HostName = config.HostName;
            }

            if (!string.IsNullOrWhiteSpace(config.HostName))
            {
                connectionFactory.Port = config.Port;

            }

            if (!string.IsNullOrWhiteSpace(config.HostName))
            {
                connectionFactory.UserName = config.UserName;
            }

            if (!string.IsNullOrWhiteSpace(config.HostName))
            {
                connectionFactory.Password = config.Password;
            }

            if (!string.IsNullOrWhiteSpace(config.HostName))
            {
                connectionFactory.VirtualHost = config.VirtualHost;
            }

            return connectionFactory;
        }

        /// <summary>
        /// 连接阻塞时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");

            ReConnect();
        }

        /// <summary>
        /// 其他时间执行发生异常时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning($"A RabbitMQ connection throw exception：{e.Exception.Message}. Trying to re-connect...");

            ReConnect();
        }

        /// <summary>
        /// 断开连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            ReConnect();
        }

    }
}
