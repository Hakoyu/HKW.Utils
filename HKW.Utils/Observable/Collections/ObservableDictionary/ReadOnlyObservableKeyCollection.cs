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
public sealed class ReadOnlyObservableKeyCollection<TKey, TValue> : IObservableCollection<TKey>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="dictionary">可观测字典</param>
    public ReadOnlyObservableKeyCollection(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

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

    internal void InvokeEvent(NotifyCollectionChangedEventArgs args)
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
