using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain.Entities;

[Serializable]
public abstract class AggregateRoot : Entity, IAggregateRoot
{
    /// <summary>
    /// 版本号，乐观锁
    /// </summary>
    public Guid Version { get; set; }

    protected AggregateRoot()
    {
        Version = Guid.NewGuid();
    }
}

[Serializable]
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    public Guid Version { get; set; }

    protected AggregateRoot()
    {
        Version = Guid.NewGuid();
    }

    protected AggregateRoot(TKey id) : base(id)
    {
        Version = Guid.NewGuid();
    }
}

public class A : AggregateRoot<int>
{
}