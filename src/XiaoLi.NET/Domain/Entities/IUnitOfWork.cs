using System;
using System.Threading;
using System.Threading.Tasks;

namespace XiaoLi.NET.Domain;

/// <summary>
/// 工作单元
/// </summary>
public interface IUnitOfWork: IDisposable
{
    /// <summary>
    /// 持久化
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> SaveChangesAndPushDomainsAsync(CancellationToken cancellationToken = default);
}