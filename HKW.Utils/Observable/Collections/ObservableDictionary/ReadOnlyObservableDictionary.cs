using System.Collections;
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
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ReadOnlyObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    private readonly IObservableDictionary<TKey, TValue> _dictionary;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary"></param>
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
        ObservableKeys = new ReadOnlyObservableList<TKey>(
            (IObservableList<TKey>)_dictionary.ObservableKeys
        );
        ObservableValues = new ReadOnlyObservableList<TValue>(
            (IObservableList<TValue>)_dictionary.ObservableValues
        );
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

    /// <summary>
    /// 为了防止忘记显式的调用Dispose方法
    /// </summary>
    ~ReadOnlyObservableDictionary()
    {
        //必须为false
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        //必须为true
        Dispose(true);
        //通知垃圾回收器不再调用终结器
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        Dispose();
    }

    /// <summary>
    /// 非密封类可重写的Dispose方法，方便子类继承时可重写
    /// </summary>
    /// <param name="disposing">释放中</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        //清理托管资源
        if (disposing)
        {
            _dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
            _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
            _dictionary.CollectionChanged -= Dictionary_CollectionChanged;
            _dictionary.PropertyChanged -= Dictionary_PropertyChanged;
        }
        //清理非托管资源

        //告诉自己已经被释放
        _disposed = true;
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

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TKey> ObservableKeys { get; }

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TValue> ObservableValues { get; }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    TValue IObservableDictionary<TKey, TValue>.this[TKey key, bool skipCheck]
    {
        get => _dictionary[key];
        set => throw new ReadOnlyException();
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

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new ReadOnlyException();
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new ReadOnlyException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new ReadOnlyException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new ReadOnlyException();
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
        throw new ReadOnlyException();
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
