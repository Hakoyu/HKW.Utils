using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 只读可观测字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>,
        IDisposable
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    private readonly IObservableDictionary<TKey, TValue> _dictionary;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary">可观测字典</param>
    public ReadOnlyObservableDictionary(IObservableDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
        _dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        _dictionary.CollectionChanged -= Dictionary_CollectionChanged;
        _dictionary.PropertyChanged -= Dictionary_PropertyChanged;

        _dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        _dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        _dictionary.CollectionChanged += Dictionary_CollectionChanged;
        _dictionary.PropertyChanged += Dictionary_PropertyChanged;
        ObservableKeys = _dictionary.ObservableKeys;
        ObservableValues = _dictionary.ObservableValues;
    }

    private void Dictionary_DictionaryChanging(
        IObservableDictionary<TKey, TValue> sender,
        NotifyDictionaryChangeEventArgs<TKey, TValue> e
    )
    {
        DictionaryChanging?.Invoke(this, e);
    }

    private void Dictionary_DictionaryChanged(
        IObservableDictionary<TKey, TValue> sender,
        NotifyDictionaryChangeEventArgs<TKey, TValue> e
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


    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ReadOnlyObservableDictionary()
    {
        Dispose(false);
    }

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
            _dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
            _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
            _dictionary.CollectionChanged -= Dictionary_CollectionChanged;
            _dictionary.PropertyChanged -= Dictionary_PropertyChanged;
        }
        _disposed = true;
    }
    #endregion

    /// <inheritdoc/>
    public TValue this[TKey key] => _dictionary[key];

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => _dictionary.Values;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TKey> ObservableKeys { get; }

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TValue> ObservableValues { get; }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.ContainsKey(item.Key);
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
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    #region Event
    /// <inheritdoc/>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    /// <inheritdoc/>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
}
