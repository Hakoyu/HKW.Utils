﻿using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 简易的只读列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class SimpleReadOnlyList<T> : IList<T>, IList
{
    /// <inheritdoc/>
    public SimpleReadOnlyList(IEnumerable<T> collection)
    {
        if (collection is IList && collection is IList<T> list)
            _list = list;
        _list = collection.ToList();
    }

    private readonly IList<T> _list;

    #region IListT
    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public T this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public int Add(T value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(T value)
    {
        return _list.Contains(value);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int index) => _list.CopyTo(array, index);

    /// <inheritdoc/>
    public int IndexOf(T value)
    {
        return _list.IndexOf(value);
    }

    /// <inheritdoc/>
    public void Insert(int index, T value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void Remove(T value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
    #endregion

    #region IList
    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

    bool IList.IsFixedSize => true;

    bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

    object? IList.this[int index]
    {
        get => _list[index];
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IList.Contains(object? value)
    {
        return _list.Contains((T)value!);
    }

    int IList.IndexOf(object? value)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((IList)_list).CopyTo(array, index);
    }

    #endregion
}
