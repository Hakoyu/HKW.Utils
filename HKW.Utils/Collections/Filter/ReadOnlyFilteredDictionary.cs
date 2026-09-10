//using System.Collections;
//using System.Data;
//using System.Diagnostics;
//using System.Diagnostics.CodeAnalysis;
//using HKW.HKWUtils.DebugViews;
//using HKW.HKWUtils.Exceptions;
//using HKW.HKWUtils.Extensions;
//using HKW.HKWUtils.Natives;
//using HKW.HKWUtils.Observable;

//namespace HKW.HKWUtils.Collections;

///// <summary>
///// 只读过滤字典
///// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredDictionary"/></para>
///// </summary>
///// <typeparam name="TKey">键类型</typeparam>
///// <typeparam name="TValue">值类型</typeparam>
///// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
//[DebuggerDisplay("Count = {Count}")]
//[DebuggerTypeProxy(typeof(ICollectionDebugView))]
//#pragma warning disable S2436
//public class ReadOnlyFilteredDictionary<TKey, TValue, TFilteredDictionary>
//#pragma warning restore S2436
//    : IDictionary<TKey, TValue>,
//        IReadOnlyDictionary<TKey, TValue>,
//        IDictionary,
//        IFilteredCollectionWrapper<
//            KeyValuePair<TKey, TValue>,
//            IObservableDictionary<TKey, TValue>,
//            TFilteredDictionary
//        >,
//        IDisposable
//    where TKey : notnull
//    where TFilteredDictionary : IDictionary<TKey, TValue>
//{
//    private readonly IObservableDictionary<TKey, TValue> _dictionary;

//    #region Ctor
//    /// <inheritdoc/>
//    /// <param name="dictionary">字典</param>
//    /// <param name="filteredDictionary">过滤字典</param>
//    /// <param name="filter">过滤器</param>
//    public ReadOnlyFilteredDictionary(
//        IObservableDictionary<TKey, TValue> dictionary,
//        TFilteredDictionary filteredDictionary,
//        Predicate<KeyValuePair<TKey, TValue>> filter
//    )
//    {
//        ArgumentNullException.ThrowIfNull(dictionary);
//        ArgumentNullException.ThrowIfNull(filteredDictionary);
//        ArgumentNullException.ThrowIfNull(filter);
//        ArgumentException.ThrowIfReadOnlyCollection(dictionary);
//        ArgumentException.ThrowIfReadOnlyCollection(filteredDictionary);

//        _dictionary = dictionary;
//        FilteredDictionary = filteredDictionary;
//        Filter = filter;
//        FilteredDictionary.Clear();
//        foreach (var pair in dictionary.Where(x => filter(x)))
//            FilteredDictionary.Add(pair.Key, pair.Value);

//        _dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
//    }

//#pragma warning disable S3776
//    private void Dictionary_DictionaryChanged(
//#pragma warning restore S3776
//        IObservableDictionary<TKey, TValue> sender,
//        NotifyDictionaryChangeEventArgs<TKey, TValue> e
//    )
//    {
//        if (e.Action is DictionaryChangeAction.Add)
//        {
//            if (e.TryGetNewPair(out var newPair) is false)
//                return;
//            if (Filter(newPair) is false)
//                return;
//            FilteredDictionary.Add(newPair);
//        }
//        else if (e.Action is DictionaryChangeAction.Remove)
//        {
//            if (e.TryGetOldPair(out var oldPair) is false)
//                return;
//            FilteredDictionary.Remove(oldPair);
//        }
//        else if (e.Action is DictionaryChangeAction.Replace)
//        {
//            if (e.TryGetOldPair(out var oldPair) is false)
//                return;
//            if (e.TryGetNewPair(out var newPair) is false)
//                return;
//            if (Filter(newPair) is false)
//            {
//                FilteredDictionary.Remove(oldPair);
//                return;
//            }
//            FilteredDictionary[oldPair.Key] = newPair.Value;
//        }
//        else if (e.Action is DictionaryChangeAction.Clear)
//        {
//            FilteredDictionary.Clear();
//        }
//    }
//    #endregion
//    /// <inheritdoc/>
//    bool IFilteredCollectionWrapper<
//        KeyValuePair<TKey, TValue>,
//        IObservableDictionary<TKey, TValue>,
//        TFilteredDictionary
//    >.AutoFilter { get; set; } = true;

//    /// <inheritdoc/>
//    public Predicate<KeyValuePair<TKey, TValue>> Filter
//    {
//        get => field;
//        set =>
//            field = field is null
//                ? value
//                : throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <summary>
//    /// 过滤完成的字典
//    /// </summary>
//    public TFilteredDictionary FilteredDictionary { get; }

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    IObservableDictionary<TKey, TValue> ICollectionWrapper<
//        KeyValuePair<TKey, TValue>,
//        IObservableDictionary<TKey, TValue>
//    >.SourceCollection => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    TFilteredDictionary IFilteredCollectionWrapper<
//        KeyValuePair<TKey, TValue>,
//        IObservableDictionary<TKey, TValue>,
//        TFilteredDictionary
//    >.FilteredCollection => FilteredDictionary;

//    /// <inheritdoc/>
//    public void BatchUpdate(Action<IObservableDictionary<TKey, TValue>> updateAction)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public void Refresh()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    #region IDictionary
//    /// <inheritdoc/>
//    public ICollection<TKey> Keys => _dictionary.Keys;

//    /// <inheritdoc/>
//    public ICollection<TValue> Values => _dictionary.Values;

//    /// <inheritdoc/>
//    public int Count => _dictionary.Count;

//    /// <inheritdoc/>
//    public bool IsReadOnly => true;

//    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys =>
//        ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;

//    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
//        ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

//    /// <inheritdoc/>
//    public bool IsFixedSize => ((IDictionary)_dictionary).IsFixedSize;

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    ICollection IDictionary.Keys => ((IDictionary)_dictionary).Keys;

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    ICollection IDictionary.Values => ((IDictionary)_dictionary).Values;

//    /// <inheritdoc/>
//    public bool IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

//    /// <inheritdoc/>
//    public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

//    object? IDictionary.this[object key]
//    {
//        get => this[(TKey)key];
//        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public TValue this[TKey key]
//    {
//        get => _dictionary[key];
//        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool ContainsKey(TKey key)
//    {
//        return _dictionary.ContainsKey(key);
//    }

//    /// <inheritdoc/>
//    bool IDictionary<TKey, TValue>.Remove(TKey key)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
//    {
//        return _dictionary.TryGetValue(key, out value);
//    }

//    /// <inheritdoc/>
//    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool Contains(KeyValuePair<TKey, TValue> item)
//    {
//        return _dictionary.Contains(item);
//    }

//    /// <inheritdoc/>
//    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
//    {
//        _dictionary.CopyTo(array, arrayIndex);
//    }

//    /// <inheritdoc/>
//    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
//    {
//        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return ((IEnumerable)_dictionary).GetEnumerator();
//    }

//    /// <inheritdoc/>
//    void IDictionary.Add(object key, object? value)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    bool IDictionary.Contains(object key)
//    {
//        return ((IDictionary)_dictionary).Contains(key);
//    }

//    /// <inheritdoc/>
//    IDictionaryEnumerator IDictionary.GetEnumerator()
//    {
//        return ((IDictionary)_dictionary).GetEnumerator();
//    }

//    /// <inheritdoc/>
//    void IDictionary.Remove(object key)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IDictionary.Clear()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ICollection.CopyTo(Array array, int index)
//    {
//        ((ICollection)_dictionary).CopyTo(array, index);
//    }
//    #endregion

//    #region IDisposable
//    private bool _disposed;

//    /// <inheritdoc/>
//    ~ReadOnlyFilteredDictionary()
//    {
//        Dispose(false);
//    }

//    /// <inheritdoc/>
//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    /// <inheritdoc/>
//    protected virtual void Dispose(bool disposing)
//    {
//        if (_disposed)
//            return;

//        if (disposing)
//        {
//            _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
//        }

//        _disposed = true;
//    }
//    #endregion
//}
