using System;
using System.Collections.ObjectModel;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain.Entities;


[Serializable]
public abstract class AggregateRoot : Entity, IAggregateRoot
{
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
}