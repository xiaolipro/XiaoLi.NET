namespace XiaoLi.NET.Domain.Entities;

/// <summary>
/// 聚合根
/// </summary>
/// <remarks>
/// <para>聚合包装一组高度相关的对象，作为一个数据修改的单元。</para>
/// <para>聚合根是聚合最外层的实体对象，它划分了一个边界，聚合根外部的对象，不能直接访问聚合根内部对象。</para>
/// </remarks>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// 泛型聚合根，支持单主键实体
/// </summary>
/// <typeparam name="TKey">实体主键类型</typeparam>
public interface IAggregateRoot<out TKey> : IEntity<TKey>, IAggregateRoot
{
}