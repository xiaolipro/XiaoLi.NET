using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using XiaoLi.NET.Domain.Events;

namespace XiaoLi.NET.Domain.Entities;

public class HasDomainEvent : IHasDomainEvent
{
    private List<DomainEvent> _domainEvents;

    [NotMapped] 
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

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