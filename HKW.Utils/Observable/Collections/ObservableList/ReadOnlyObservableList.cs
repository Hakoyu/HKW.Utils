using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读的可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyObservableList<T>
    : IObservableList<T>,
        IReadOnlyObservableList<T>,
        IList,
        IDisposable,
        ICollectionNotificationSnapshot,
        ICollectionNotificationState
{
    private readonly IObservableList<T> _list;

    private readonly CollectionNotificationScope<
        ObservableListChangingEventHandler<T>,
        ObservableListChangedEventHandler<T>
    > _notifications = new();

    bool ICollectionNotificationState.IsNotifying =>
        _notifications.IsActive || _list is ICollectionNotificationState { IsNotifying: true };

    bool ICollectionNotificationSnapshot.CaptureNotifications()
    {
        if (_notifications.IsActive)
            return false;
        _notifications.Enter(ListChanging, ListChanged, CollectionChanged, PropertyChanged);
        return true;
    }

    void ICollectionNotificationSnapshot.ReleaseNotifications()
    {
        _notifications.Exit();
    }

    #region Ctor

    /// <inheritdoc/>
    /// <param name="list">引用列表</param>
    public ReadOnlyObservableList(IObservableList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        _list = list;

        _list.ListChanging += List_ListChanging;
        _list.ListChanged += List_ListChanged;
        _list.CollectionChanged += List_CollectionChanged;
        _list.PropertyChanged += List_PropertyChanged;
    }

    private void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
    {
        (_notifications.IsActive ? _notifications.Changing : ListChanging)?.Invoke(this, e);
    }

    private void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
    {
        (_notifications.IsActive ? _notifications.Changed : ListChanged)?.Invoke(this, e);
    }

    private void List_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        (_notifications.IsActive ? _notifications.Collection : CollectionChanged)?.Invoke(this, e);
    }

    private void List_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        (_notifications.IsActive ? _notifications.Property : PropertyChanged)?.Invoke(this, e);
    }
    #endregion

    #region Dispose
    private bool _disposed;

    /// <summary>
    /// 解除源事件订阅; 释放后仍可读取源内容, 当前操作已捕获的通知仍会完成.
    /// </summary>
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
            _list.ListChanging -= List_ListChanging;
            _list.ListChanged -= List_ListChanged;
            _list.CollectionChanged -= List_CollectionChanged;
            _list.PropertyChanged -= List_PropertyChanged;
        }
        _disposed = true;
    }
    #endregion

    #region IListT

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public T this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    void IList<T>.Insert(int index, T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
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

    bool IList.IsFixedSize => true;

    bool ICollection.IsSynchronized => CollectionInterfaceBridge.IsSynchronized(_list);

    object ICollection.SyncRoot => CollectionInterfaceBridge.SyncRoot(_list, this);

    object? IList.this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IList.Clear()
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

    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CollectionInterfaceBridge.CopyTo(_list, array, index);
    }

    #endregion IObservableListT

    #region Event
    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<T>? ListChanging;

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<T>? ListChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
}
