using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 简易的非通用单个项目的只读列表
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class SingleItemReadOnlyList<T> : IList<T>, IReadOnlyList<T>, IList
{
    /// <inheritdoc/>
    public SingleItemReadOnlyList(T item) => _item = item;

    private readonly T _item;

    #region IListT
    /// <inheritdoc/>
    public T this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0);
            return _item;
        }
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public int Count => 1;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList<T>.Insert(int index, T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList<T>.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _item?.Equals(item) is true;
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
    bool IList.IsFixedSize => true;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    object? IList.this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0);
            return _item;
        }
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IList.Contains(object? value)
    {
        return _item?.Equals(value) is true;
    }

    int IList.IndexOf(object? value)
    {
        return _item?.Equals(value) is true ? 0 : -1;
    }

    void ICollection.CopyTo(Array array, int index)
    {
        array.SetValue(_item, index);
    }

    #endregion
}
