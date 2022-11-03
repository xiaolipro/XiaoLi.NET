using System;
using System.Collections.Generic;
using System.Diagnostics;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain
{
    /// <summary>
    /// 实体
    /// </summary>
    public abstract class Entity
    {
        private int? _requestHashCode;

        private List<Event> _domainEvents;

        public virtual int Id { get; protected set; }

        public bool IsTransient => Id == default;

        /// <summary>
        /// 领域事件集合
        /// </summary>
        public IReadOnlyCollection<Event> DomainEvents => _domainEvents?.AsReadOnly();

        #region 领域事件

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="event">领域事件</param>
        public void AddDomainEvent(Event @event)
        {
            _domainEvents ??= new List<Event>();
            _domainEvents.Add(@event);
        }

        /// <summary>
        /// 移除领域事件
        /// </summary>
        /// <param name="event">领域事件</param>
        public void RemoveDomainEvent(Event @event)
        {
            _domainEvents?.Remove(@event);
        }

        /// <summary>
        /// 移除所有领域事件
        /// </summary>
        public void ClearDomainEvent()
        {
            _domainEvents?.Clear();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is not Entity entity) return false;

            if (object.ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;

            if (IsTransient || entity.IsTransient) return false;

            return Id == entity.Id;
        }

        public override int GetHashCode()
        {
            if (IsTransient) return base.GetHashCode();
            
            if (!_requestHashCode.HasValue)
            {
                var hashCode = Id.GetHashCode();
                unchecked
                {
                    hashCode = (hashCode * 131) ^ 31;
                }

                _requestHashCode = hashCode;
            }

            Debug.Assert(_requestHashCode != null);
            return _requestHashCode.Value;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null) return true;

            return a != null && a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }
    }
}