using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测的只读值集合
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyObservableValueCollection<TKey, TValue>
    : IObservableCollection<TValue>,
        IReadOnlyObservableCollection<TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="dictionary">可观测字典</param>
    public ReadOnlyObservableValueCollection(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    private readonly IDictionary<TKey, TValue> _dictionary;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    void ICollection<TValue>.Add(TValue item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<TValue>.Remove(TValue item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<TValue>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(TValue item)
    {
        return _dictionary.Values.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TValue[] array, int arrayIndex)
    {
        _dictionary.Values.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="args">事件参数</param>
    public void InvokeEvent(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
        if (args.Action is not NotifyCollectionChangedAction.Replace)
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
