using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using XiaoLi.NET.Domain.Events;
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
    
    private List<DomainEvent> _domainEvents;

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents  => _domainEvents?.AsReadOnly();
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents ??= new List<DomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }

    protected void ClearDomainEvents()
    {
        _domainEvents?.Clear();
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
    
    private List<DomainEvent> _domainEvents;

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents  => _domainEvents?.AsReadOnly();
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents ??= new List<DomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }

    protected void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}