using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察的过滤字典
/// <para>基于 <see cref="FilteredDictionaryWrapper{TKey, TValue, TDictionary, TFilteredDictionary}"/> 维护一个实时过滤的字典</para>
/// <para>源字典发生变更时，会自动同步更新 <see cref="FilteredDictionaryWrapper{TKey, TValue, TDictionary, TFilteredDictionary}.FilteredDictionary"/></para>
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
#pragma warning disable S2436
public sealed class ObservableFilteredDictionaryWrapper<
    TKey,
    TValue,
    TDictionary,
    TFilteredDictionary
>
#pragma warning restore S2436
    : FilteredDictionaryWrapper<TKey, TValue, TDictionary, TFilteredDictionary>, IDisposable
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>, INotifyCollectionChanged
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    private bool _isUpdating;
    private bool _disposed;

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="filteredDictionary">过滤字典</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public ObservableFilteredDictionaryWrapper(
        TDictionary dictionary,
        TFilteredDictionary filteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter,
        bool autoFilter = true
    )
        : base(dictionary, filteredDictionary, filter, autoFilter)
    {
        SourceDictionary.CollectionChanged += OnSourceDictionaryCollectionChanged;
    }

    /// <inheritdoc/>
    public override TValue this[TKey key]
    {
        get => base[key];
        set
        {
            _isUpdating = true;
            try
            {
                base[key] = value;
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }

    /// <inheritdoc/>
    public override void BatchUpdate(Action<TDictionary> updateAction)
    {
        _isUpdating = true;
        try
        {
            base.BatchUpdate(updateAction);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override bool TryAdd(TKey key, TValue value)
    {
        _isUpdating = true;
        try
        {
            return base.TryAdd(key, value);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    protected override void Add(KeyValuePair<TKey, TValue> item)
    {
        _isUpdating = true;
        try
        {
            base.Add(item);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    protected override bool Remove(KeyValuePair<TKey, TValue> item)
    {
        _isUpdating = true;
        try
        {
            return base.Remove(item);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override void Clear()
    {
        _isUpdating = true;
        try
        {
            base.Clear();
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <summary>
    /// 释放资源并取消订阅源字典事件
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        SourceDictionary.CollectionChanged -= OnSourceDictionaryCollectionChanged;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void OnSourceDictionaryCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        if (_isUpdating)
            return;

        if (e.Action == NotifyCollectionChangedAction.Reset && SourceDictionary.Count == 0)
        {
            FilteredDictionary.Clear();
            return;
        }

        if (AutoFilter is false)
            return;

        Refresh();
    }
}
