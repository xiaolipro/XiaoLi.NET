using XiaoLi.NET.Domain.Entities;

namespace XiaoLi.NET.Domain.Repositories;

/// <summary>
/// 基础查询仓储
/// </summary>
/// <typeparam name="TAggregateRoot">聚合根</typeparam>
public interface IBasicQueryableRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
    
}