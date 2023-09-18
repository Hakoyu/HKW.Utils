using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 简易的非通用单个项目的只读列表
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public sealed class SimpleSingleItemReadOnlyList<T> : IList<T>, IList
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly T _item;

    /// <inheritdoc/>
    public SimpleSingleItemReadOnlyList(T item) => _item = item;

    #region IListT
    /// <inheritdoc/>
    public T this[int index]
    {
        get
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _item;
        }
        set => throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public int Count => 1;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public void Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _item is null ? item is null : _item.Equals(item);
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return Contains(item) ? 0 : -1;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        array.SetValue(_item, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        yield return _item;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    #region IList
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsFixedSize => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => false;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => this;

    object? IList.this[int index]
    {
        get
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _item;
        }
        set => throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    bool IList.Contains(object? value)
    {
        return _item is null ? value is null : _item.Equals(value);
    }

    int IList.IndexOf(object? value)
    {
        return (_item is null ? value is null : _item.Equals(value)) ? 0 : -1;
    }

    void ICollection.CopyTo(Array array, int index)
    {
        array.SetValue(_item, index);
    }

    #endregion
}
