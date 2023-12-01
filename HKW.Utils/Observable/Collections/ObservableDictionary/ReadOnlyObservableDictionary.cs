using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观察字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>,
        IDisposable
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IObservableDictionary<TKey, TValue> _dictionary;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary"></param>
    public ReadOnlyObservableDictionary(IObservableDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
        _dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        _dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        _dictionary.CollectionChanged += Dictionary_CollectionChanged;
        _dictionary.PropertyChanged += Dictionary_PropertyChanged;
    }

    private void Dictionary_DictionaryChanging(
        IObservableDictionary<TKey, TValue> sender,
        NotifyDictionaryChangingEventArgs<TKey, TValue> e
    )
    {
        DictionaryChanging?.Invoke(this, e);
    }

    private void Dictionary_DictionaryChanged(
        IObservableDictionary<TKey, TValue> sender,
        NotifyDictionaryChangedEventArgs<TKey, TValue> e
    )
    {
        DictionaryChanged?.Invoke(this, e);
    }

    private void Dictionary_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void Dictionary_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
    #endregion

    #region Dispose
    private bool _disposedValue;

    /// <inheritdoc/>
    ~ReadOnlyObservableDictionary() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (disposing)
            Close();
        _disposedValue = true;
    }

    /// <inheritdoc/>
    public void Close()
    {
        _dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        _dictionary.CollectionChanged -= Dictionary_CollectionChanged;
        _dictionary.PropertyChanged -= Dictionary_PropertyChanged;
    }
    #endregion
    /// <inheritdoc/>
    public TValue this[TKey key] => ((IReadOnlyDictionary<TKey, TValue>)_dictionary)[key];

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<TKey> IObservableDictionary<TKey, TValue>.Comparer => _dictionary.Comparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableCollection<KeyValuePair<TKey, TValue>>.TriggerRemoveActionOnClear
    {
        get => _dictionary.TriggerRemoveActionOnClear;
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    IList<KeyValuePair<TKey, TValue>> IObservableDictionary<TKey, TValue>.TryAddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> items
    )
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IObservableDictionary<TKey, TValue>.AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
        KeyValuePair<TKey, TValue>[] array,
        int arrayIndex
    )
    {
        _dictionary.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    #region Event
    /// <inheritdoc/>
    /// <remarks>!!!注意!!! 使用 <see cref="CancelEventArgs.Cancel"/> 不会产生效果</remarks>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    /// <inheritdoc/>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
}
