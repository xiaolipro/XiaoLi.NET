using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.Domain
{
    /// <summary>
    /// 实体基类
    /// </summary>
    /// <typeparam name="TKey">实体唯一标识类型</typeparam>
    public abstract class Entity<TKey>
    {
        /// <summary>
        /// 实体唯一标识
        /// </summary>
        [Required]
        public TKey Id { get; protected set; }

        private int? _requestHashCode;

        private List<Event> _domainEvents;

        public IReadOnlyCollection<Event> DomainEvents => _domainEvents?.AsReadOnly();
        
        /// <summary>
        /// 实体刚创建时，Id默认是初始值
        /// </summary>
        public bool IsTransient => Id.Equals(default(TKey));

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

        #region 相等性比较

        public override bool Equals(object obj)
        {
            if (obj is not Entity<TKey> entity) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;

            if (IsTransient || entity.IsTransient) return false;

            return Id.Equals(entity.Id);
        }

        public static bool operator ==(Entity<TKey> a, Entity<TKey> b)
        {
            if (a is null) return b is null;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TKey> a, Entity<TKey> b)
        {
            return !(a == b);
        }

        #endregion

        public override int GetHashCode()
        {
            if (IsTransient) return base.GetHashCode();

            if (!_requestHashCode.HasValue)
            {
                unchecked
                {
                    // 素数随机分布
                    _requestHashCode = Id.GetHashCode() ^ 31;
                }
            }

            Debug.Assert(_requestHashCode != null);
            return _requestHashCode.Value;
        }
    }
}