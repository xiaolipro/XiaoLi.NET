using System;
using System.Collections.Generic;
using XiaoLi.NET.Domain.Events;

namespace XiaoLi.NET.Domain.Entities;

/*
 * 聚合包装一组高度相关的对象，作为一个数据修改的单元。
 * 聚合根是聚合最外层的实体对象，它划分了一个边界，聚合根外部的对象，不能直接访问聚合根内部对象。
 */

/// <summary>
/// 定义聚合根，复合主键下用这个
/// 其他情况尽可能使用 <see cref="IAggregateRoot{TKey}"/>
/// </summary>
public interface IAggregateRoot : IEntity
{
    public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}

/// <summary>
/// 使用"Id"做主键的聚合根
/// </summary>
/// <typeparam name="TKey">实体主键类型</typeparam>
public interface IAggregateRoot<out TKey> : IEntity<TKey>, IAggregateRoot
{
}