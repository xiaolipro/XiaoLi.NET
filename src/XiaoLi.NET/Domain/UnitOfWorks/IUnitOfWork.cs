using System.Threading;
using System.Threading.Tasks;

namespace XiaoLi.NET.Domain.UnitOfWorks;

/// <summary>
/// 工作单元
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// 将上下文所有更改保存到数据库
    /// </summary>
    /// <param name="pushDomainEvens">推送领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task SaveChangesAsync(bool pushDomainEvens = true, CancellationToken cancellationToken = default);
}