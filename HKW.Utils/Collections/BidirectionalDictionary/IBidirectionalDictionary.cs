using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

#pragma warning disable CS1584,CS1658

/// <summary>
/// 双向字典接口
/// </summary>
/// <typeparam name="T1">类型1</typeparam>
/// <typeparam name="T2">类型2</typeparam>
public interface IBidirectionalDictionary<T1, T2> : IDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    /// <inheritdoc cref="IDictionary{T1, T2}.this[T1]"/>
    public new T2 this[T1 key1] { get; set; }

    /// <inheritdoc cref="IDictionary{T2, T1}.this[T2]"/>
    public T1 this[T2 key2] { get; set; }

    /// <summary>
    /// 字典1 (只读)
    /// </summary>
    public IDictionary<T1, T2> Dictionary1 { get; }

    /// <summary>
    /// 字典2 (只读)
    /// </summary>
    public IDictionary<T2, T1> Dictionary2 { get; }

    /// <inheritdoc cref="Dictionary{T1, T2}.TryAdd(T1, T2)"/>
    public bool TryAdd(T1 key1, T2 value2);

    /// <inheritdoc cref="Dictionary{T2, T1}.TryAdd(T2, T1)"/>
    public bool TryAdd(T2 key2, T1 value1);

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key1">键</param>
    /// <param name="value2">值</param>
    /// <returns>是否设置成功</returns>
    /// <remarks>
    /// <para>
    /// 1. 当 key 和 value 都不存在时, 添加新值, 返回 true.
    /// </para>
    /// <para>
    /// 2. 当 key 存在 value 不存在时, 替换 dic1 的 value, 删除 dic2 的 value 再添加新的 (value, key), 返回 true.
    /// </para>
    /// <para>
    /// 3. 当 key 不存在 value 存在时, 返回 false. 基于仅依据 key 替换 value 的原则不予替换, 可以使用 TrySetValue(T2,T1)
    /// </para>
    /// <para>
    /// 4. 当 key 与 value 已组成现有映射时, 返回 true.
    /// </para>
    /// </remarks>
    public bool TrySetValue(T1 key1, T2 value2);

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key2">键</param>
    /// <param name="value1">值</param>
    /// <returns>是否设置成功</returns>
    /// <remarks>
    /// <para>
    /// 1. 当 key 和 value 都不存在时, 添加新值, 返回 true.
    /// </para>
    /// <para>
    /// 2. 当 key 存在 value 不存在时, 替换 dic2 的 value, 删除 dic1 的 value 再添加新的 (value, key), 返回 true.
    /// </para>
    /// <para>
    /// 3. 当 key 不存在 value 存在时, 返回 false. 基于仅依据 key 替换 value 的原则不予替换, 可以使用 TrySetValue(T1,T2)
    /// </para>
    /// <para>
    /// 4. 当 key 与 value 已组成现有映射时, 返回 true.
    /// </para>
    /// </remarks>
    public bool TrySetValue(T2 key2, T1 value1);

    /// <inheritdoc cref="IDictionary{T1, T2}.Remove(T1)"/>
    public new bool Remove(T1 key1);

    /// <inheritdoc cref="IDictionary{T2, T1}.Remove(T2)"/>
    public bool Remove(T2 key2);

    /// <inheritdoc cref="ICollection{KeyValuePair{T1, T2}}.Remove(KeyValuePair{T1, T2})"/>
    public new bool Remove(KeyValuePair<T1, T2> item1);

    /// <inheritdoc cref="ICollection{KeyValuePair{T2, T1}}.Remove(KeyValuePair{T2, T1})"/>
    public bool Remove(KeyValuePair<T2, T1> item2);

    /// <inheritdoc cref="IDictionary{T1, T2}.ContainsKey(T1)"/>
    public new bool ContainsKey(T1 key1);

    /// <inheritdoc cref="IDictionary{T2, T1}.ContainsKey(T2)"/>
    public bool ContainsKey(T2 key2);

    /// <inheritdoc cref="ICollection{KeyValuePair{T1, T2}}.Contains(KeyValuePair{T1, T2})"/>
    public new bool Contains(KeyValuePair<T1, T2> item1);

    /// <inheritdoc cref="ICollection{KeyValuePair{T2, T1}}.Contains(KeyValuePair{T2, T1})"/>
    public bool Contains(KeyValuePair<T2, T1> item2);

    /// <inheritdoc cref="ICollection{KeyValuePair{T1, T2}}.CopyTo(KeyValuePair{T1, T2}[], int)"/>
    public new void CopyTo(KeyValuePair<T1, T2>[] array1, int arrayIndex);

    /// <inheritdoc cref="ICollection{KeyValuePair{T2, T1}}.CopyTo(KeyValuePair{T2, T1}[], int)"/>
    public void CopyTo(KeyValuePair<T2, T1>[] array2, int arrayIndex);

    /// <inheritdoc cref="IDictionary{T1, T2}.TryGetValue(T1, out T2)"/>
    public new bool TryGetValue(T1 key1, [MaybeNullWhen(false)] out T2 value);

    /// <inheritdoc cref="IDictionary{T2, T1}.TryGetValue(T2, out T1)"/>
    public bool TryGetValue(T2 key2, [MaybeNullWhen(false)] out T1 value);
}
#pragma warning restore CS1584, CS1658
