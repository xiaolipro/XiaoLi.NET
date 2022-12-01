using System.Collections.Generic;
using XiaoLi.NET.Domain.Events;

namespace XiaoLi.NET.Domain.Entities;

public interface IHasDomainEvent
{
    public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
}