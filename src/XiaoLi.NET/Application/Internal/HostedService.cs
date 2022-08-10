using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Application.Internal
{
    /// <summary>
    /// 主机托管服务
    /// </summary>
    internal class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> _logger;

        public HostedService(IHost host, ILogger<HostedService> logger)
        {
            Debug.Assert(host.Services != null);
            // 初始化根服务
            InternalApp.ServiceProvider = host.Services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("服务启动中");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("服务已关闭");
            return Task.CompletedTask;
        }
    }
}