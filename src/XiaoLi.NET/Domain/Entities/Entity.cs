using System;
using System.Collections.Generic;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain.Entities;

[Serializable]
public abstract class Entity : IEntity
{
    private readonly List<Event> _localEvents = new();
    private readonly List<Event> _distributedEvents = new();

    public IReadOnlyCollection<Event> LocalEvents => _localEvents?.AsReadOnly();
    public IReadOnlyCollection<Event> DistributedEvents => _distributedEvents?.AsReadOnly();
    
    public abstract object[] GetKeys();
    
    public override string ToString()
    {
        return $"[实体: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }
    
    public bool IsTransient => EntityHelper.IsDefaultKeys(this);

    public bool EntityEquals(IEntity other)
    {
        return EntityHelper.EntityEquals(this, other);
    }
    
    public virtual void ClearLocalEvents()
    {
        _localEvents.Clear();
    }

    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }

    protected virtual void AddLocalEvent(Event @event)
    {
        _localEvents.Add(@event);
    }

    protected virtual void AddDistributedEvent(Event @event)
    {
        _distributedEvents.Add(@event);
    }
    
}

/// <summary>
/// 实体抽象
/// </summary>
/// <typeparam name="TKey">实体唯一标识类型</typeparam>
[Serializable]
public abstract class Entity<TKey>: Entity, IEntity<TKey>
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    public virtual TKey Id { get; protected set; }

    protected Entity()
    {

    }
    protected Entity(TKey id)
    {
        Id = id;
    }
    
    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
    
    public override string ToString()
    {
        return $"[实体: {GetType().Name}] Id = {Id}";
    }
}