using XiaoLi.NET.Domain.Entities;

namespace XiaoLi.NET.Domain.Repositories;

/// <summary>
/// 仓储，关注于单一聚合的持久化
/// </summary>
/// <typeparam name="TAggregateRoot">聚合根</typeparam>
public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
{
}

/// <summary>
/// 仓储，关注于单一聚合的持久化
/// </summary>
/// <typeparam name="TAggregateRoot">聚合根</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<TAggregateRoot, TKey> where TAggregateRoot : IAggregateRoot<TKey>
{
}