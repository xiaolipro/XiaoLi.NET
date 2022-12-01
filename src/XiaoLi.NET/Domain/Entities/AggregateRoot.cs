using System;
using System.Collections.ObjectModel;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain.Entities;


[Serializable]
public abstract class AggregateRoot : HasDomainEvent, IAggregateRoot
{
    public abstract object[] GetKeys();
    public Guid Version { get; set; }

    protected AggregateRoot()
    {
        Version = Guid.NewGuid();
    }
}

[Serializable]
public abstract class AggregateRoot<TKey> : HasDomainEvent, IAggregateRoot<TKey>
{
    public abstract object[] GetKeys();
    public abstract TKey Id { get; }
    public Guid Version { get; set; }

    protected AggregateRoot()
    {
        Version = Guid.NewGuid();
    }
}