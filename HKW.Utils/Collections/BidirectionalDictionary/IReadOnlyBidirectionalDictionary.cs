using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

#pragma warning disable CS1584,CS1658

/// <summary>
/// 只读双向字典接口
/// </summary>
/// <typeparam name="T1">类型1</typeparam>
/// <typeparam name="T2">类型2</typeparam>
public interface IReadOnlyBidirectionalDictionary<T1, T2> : IReadOnlyDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    /// <inheritdoc cref="IDictionary{T1, T2}.this[T1]"/>
    public new T2 this[T1 key1] { get; }

    /// <inheritdoc cref="IDictionary{T2, T1}.this[T2]"/>
    public T1 this[T2 key2] { get; }

    /// <summary>
    /// 字典1 (只读)
    /// </summary>
    public IReadOnlyDictionary<T1, T2> Dictionary1 { get; }

    /// <summary>
    /// 字典2 (只读)
    /// </summary>
    public IReadOnlyDictionary<T2, T1> Dictionary2 { get; }

    /// <inheritdoc cref="IDictionary{T1, T2}.ContainsKey(T1)"/>
    public new bool ContainsKey(T1 key1);

    /// <inheritdoc cref="IDictionary{T2, T1}.ContainsKey(T2)"/>
    public bool ContainsKey(T2 key2);

    /// <inheritdoc cref="ICollection{KeyValuePair{T1, T2}}.Contains(KeyValuePair{T1, T2})"/>
    public bool Contains(KeyValuePair<T1, T2> item1);

    /// <inheritdoc cref="ICollection{KeyValuePair{T2, T1}}.Contains(KeyValuePair{T2, T1})"/>
    public bool Contains(KeyValuePair<T2, T1> item2);

    /// <inheritdoc cref="IDictionary{T1, T2}.TryGetValue(T1, out T2)"/>
    public new bool TryGetValue(T1 key1, [MaybeNullWhen(false)] out T2 value);

    /// <inheritdoc cref="IDictionary{T2, T1}.TryGetValue(T2, out T1)"/>
    public bool TryGetValue(T2 key2, [MaybeNullWhen(false)] out T1 value);

    /// <summary>
    /// 获取正向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public T2 GetForward(T1 key);

    /// <summary>
    /// 获取反向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public T1 GetReverse(T2 key);

    /// <summary>
    /// 检查正向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsKeyForward(T1 key);

    /// <summary>
    /// 检查反向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsKeyReverse(T2 key);

    /// <summary>
    /// 检查正向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsForward(KeyValuePair<T1, T2> item);

    /// <summary>
    /// 检查反向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsReverse(KeyValuePair<T2, T1> item);

    /// <summary>
    /// 尝试获取正向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryGetForward(T1 key, [MaybeNullWhen(false)] out T2 value);

    /// <summary>
    /// 尝试获取反向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryGetReverse(T2 key, [MaybeNullWhen(false)] out T1 value);
}
#pragma warning restore CS1584, CS1658
