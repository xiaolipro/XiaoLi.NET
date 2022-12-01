using XiaoLi.NET.Domain.Entities;
using XiaoLi.NET.Domain.SeedWork;

namespace XiaoLi.NET.Domain;

public interface IRepository<TEntity> where TEntity : AggregateRoot<TEntity>
{
    IUnitOfWork UnitOfWork { get; }
}