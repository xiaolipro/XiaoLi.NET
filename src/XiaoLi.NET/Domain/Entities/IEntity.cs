using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace XiaoLi.NET.Domain.Entities;

/// <summary>
/// 实体顶层抽象
/// 支持复合索引，单主键建议使用<see cref="IEntity{TKey}"/>
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
/// 实体顶层泛型抽象
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntity<out TKey>: IEntity
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    TKey Id { get; }
}