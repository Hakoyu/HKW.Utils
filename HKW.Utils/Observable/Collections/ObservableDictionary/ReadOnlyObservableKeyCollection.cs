using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测的只读键集合
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyObservableKeyCollection<TKey, TValue>
    : IObservableCollection<TKey>,
        IReadOnlyObservableCollection<TKey>,
        ICollectionNotificationSnapshot,
        ICollectionNotificationState
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="dictionary">可观测字典</param>
    public ReadOnlyObservableKeyCollection(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    private readonly CollectionNotificationScope<Delegate, Delegate> _notifications = new();

    bool ICollectionNotificationState.IsNotifying =>
        _notifications.IsActive
        || _dictionary is ICollectionNotificationState { IsNotifying: true };
    internal bool HasCollectionObservers =>
        _notifications.IsActive
            ? _notifications.Collection is not null
            : CollectionChanged is not null;

    bool ICollectionNotificationSnapshot.CaptureNotifications()
    {
        if (_notifications.IsActive)
            return false;
        _notifications.Enter(null, null, CollectionChanged, PropertyChanged);
        return true;
    }

    void ICollectionNotificationSnapshot.ReleaseNotifications() => _notifications.Exit();

    private readonly IDictionary<TKey, TValue> _dictionary;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    void ICollection<TKey>.Add(TKey item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<TKey>.Remove(TKey item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<TKey>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(TKey item)
    {
        return _dictionary.ContainsKey(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TKey[] array, int arrayIndex)
    {
        _dictionary.Keys.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TKey> GetEnumerator()
    {
        return _dictionary.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal void Notify(
        NotifyCollectionChangedAction action,
        TKey? newItem = default,
        TKey? oldItem = default,
        int index = -1
    )
    {
        var collection = _notifications.IsActive ? _notifications.Collection : CollectionChanged;
        var property = _notifications.IsActive ? _notifications.Property : PropertyChanged;
        if (collection is not null)
        {
            var args = action switch
            {
                NotifyCollectionChangedAction.Add => new NotifyCollectionChangedEventArgs(
                    action,
                    newItem,
                    index
                ),
                NotifyCollectionChangedAction.Remove => new NotifyCollectionChangedEventArgs(
                    action,
                    oldItem,
                    index
                ),
                NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(
                    action,
                    newItem,
                    oldItem,
                    index
                ),
                _ => NotifyCollectionChangedEventArgs.Cache_Reset,
            };
            collection(this, args);
        }
        if (action is not NotifyCollectionChangedAction.Replace)
            property?.Invoke(this, PropertyChangedArgsCache.Count);
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="args">事件参数</param>
    public void InvokeEvent(NotifyCollectionChangedEventArgs args)
    {
        (_notifications.IsActive ? _notifications.Collection : CollectionChanged)?.Invoke(
            this,
            args
        );
        if (args.Action is not NotifyCollectionChangedAction.Replace)
            (_notifications.IsActive ? _notifications.Property : PropertyChanged)?.Invoke(
                this,
                PropertyChangedArgsCache.Count
            );
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
