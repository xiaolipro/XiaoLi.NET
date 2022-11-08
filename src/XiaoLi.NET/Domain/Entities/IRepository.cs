using XiaoLi.NET.Domain.Entities;
using XiaoLi.NET.Domain.SeedWork;

namespace XiaoLi.NET.Domain;

public interface IRepository<TEntity, in TKey> where TEntity : AggregateRoot<TKey>
{
    IUnitOfWork UnitOfWork { get; }
}