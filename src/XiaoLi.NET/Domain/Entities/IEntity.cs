namespace XiaoLi.NET.Domain.Entities;

/// <summary>
/// 实体顶层抽象，支持复合索引
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 获取所有主键
    /// </summary>
    /// <returns></returns>
    object[] GetKeys();
}

/// <summary>
/// 实体顶层泛型抽象，索引唯一
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntity<out TKey>: IEntity
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    TKey Id { get; }
}