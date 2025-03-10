﻿using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集成只读列表的列表
/// <para>
/// 示例:
/// <code><![CDATA[
/// IntegratedReadOnlyList<int, List<int>, ReadOnlyCollection<int>> List { get; } = new(new (), l => new (l));
/// ReadOnlyCollection<int> ReadOnlyList => List.ReadOnlyList;
/// ]]></code>
/// </para>
/// </summary>
/// <typeparam name="T">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
/// <typeparam name="TReadOnlyList">只读列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class IntegratedReadOnlyList<T, TList, TReadOnlyList> : IList<T>, IReadOnlyList<T>
    where TList : IList<T>
    where TReadOnlyList : IReadOnlyList<T>
{
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="readOnlyList">只读列表</param>
    public IntegratedReadOnlyList(TList list, TReadOnlyList readOnlyList)
    {
        List = list;
        ReadOnlyList = readOnlyList;
    }

    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="getReadOnlyList">获取只读列表</param>
    public IntegratedReadOnlyList(TList list, Func<TList, TReadOnlyList> getReadOnlyList)
        : this(list, getReadOnlyList(list)) { }

    /// <summary>
    /// 列表
    /// </summary>
    public TList List { get; }

    /// <summary>
    /// 只读列表
    /// </summary>
    public TReadOnlyList ReadOnlyList { get; }

    #region IList
    /// <inheritdoc/>
    public T this[int index]
    {
        get => List[index];
        set => List[index] = value;
    }

    /// <inheritdoc/>
    public int Count => List.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => List.IsReadOnly;

    /// <inheritdoc/>
    public void Add(T item)
    {
        List.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        List.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return List.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        List.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return List.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        List.Insert(index, item);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return List.Remove(item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        List.RemoveAt(index);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)List).GetEnumerator();
    }
    #endregion
}
