using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ReadOnlyList<T> : IList<T>, IReadOnlyList<T>, IList
{
    /// <inheritdoc/>
    public ReadOnlyList(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (collection is IList<T> list)
            _list = list;
        else
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
    void IList<T>.Insert(int index, T value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList<T>.RemoveAt(int index)
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
    object ICollection.SyncRoot => CollectionInterfaceBridge.SyncRoot(_list, this);

    bool IList.IsFixedSize => true;

    bool ICollection.IsSynchronized => CollectionInterfaceBridge.IsSynchronized(_list);

    object? IList.this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IList.Contains(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<T>(value) && Contains((T)value!);
    }

    int IList.IndexOf(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<T>(value) ? IndexOf((T)value!) : -1;
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CollectionInterfaceBridge.CopyTo(_list, array, index);
    }

    #endregion
}
