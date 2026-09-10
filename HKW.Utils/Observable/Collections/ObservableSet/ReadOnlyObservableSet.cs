using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观测合集
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyObservableSet<T>
    : IObservableSet<T>,
        IReadOnlyObservableSet<T>,
        IDisposable
{
    private readonly IObservableSet<T> _set;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    public ReadOnlyObservableSet(IObservableSet<T> set)
    {
        _set = set;
        _set.SetChanging += Set_SetChanging;
        _set.SetChanged += Set_SetChanged;
        _set.CollectionChanged += Set_CollectionChanged;
        _set.PropertyChanged += Set_PropertyChanged;
    }

    private void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
    {
        SetChanging?.Invoke(this, e);
    }

    private void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
    {
        SetChanged?.Invoke(this, e);
    }

    private void Set_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void Set_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
    #endregion

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ReadOnlyObservableSet() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _set.SetChanging -= Set_SetChanging;
            _set.SetChanged -= Set_SetChanged;
            _set.CollectionChanged -= Set_CollectionChanged;
            _set.PropertyChanged -= Set_PropertyChanged;
        }
        _disposed = true;
    }
    #endregion

    #region IReadOnlyObservableSet
    /// <inheritdoc/>
    public int Count => _set.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _set.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _set.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _set.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _set.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return _set.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return _set.SetEquals(other);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_set).GetEnumerator();
    }
    #endregion

    #region IObservableSet

    bool ISet<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Event
    /// <inheritdoc/>
    public event ObservableSetChangingEventHandler<T>? SetChanging;

    /// <inheritdoc/>
    public event ObservableSetChangedEventHandler<T>? SetChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
}
