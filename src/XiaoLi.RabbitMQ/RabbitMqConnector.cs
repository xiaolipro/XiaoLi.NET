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
using XiaoLi.RabbitMQ.Configs;

namespace XiaoLi.RabbitMQ
{
    public class RabbitMQConnector : IRabbitMQConnector
    {
        private readonly ILogger<RabbitMQConnector> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _retries;
        private IConnection _connection;
        private bool _disposed;
        private readonly object _lock = new object();

        
        public RabbitMQConnector(ILogger<RabbitMQConnector> logger, IOptions<RabbitMQConfig> config)
        {
            _logger = logger;
            _connectionFactory = GetConnectionFactory(config.Value);
            _retries = config.Value.Retries;
        }


        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_lock)
            {
                // 当出现socket异常、broker不可达异常时
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
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

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;

                    // RabbitMQ出于自身保护策略，通过阻塞方式限制写入，导致了生产者应用“假死”，不再对外服务。
                    // 比若说CPU/IO/RAM资源下降，队列堆积，导致堵塞，就会触发这个事件
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation(
                        "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                        _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                TryConnect();
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



        #region private methods

        private IConnectionFactory GetConnectionFactory(RabbitMQConfig config)
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

            TryConnect();
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

            TryConnect();
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

            TryConnect();
        }

        #endregion
    }
}
