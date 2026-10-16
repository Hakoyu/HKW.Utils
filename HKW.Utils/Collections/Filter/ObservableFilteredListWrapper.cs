using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察的过滤列表
/// <para>基于 <see cref="FilteredListWrapper{TItem, TList, TFilteredList}"/> 维护一个实时过滤的列表</para>
/// <para>源列表发生变更时，会自动同步更新 <see cref="FilteredListWrapper{TItem, TList, TFilteredList}.FilteredList"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
#pragma warning disable S2436
public sealed class ObservableFilteredListWrapper<TItem, TList, TFilteredList>
#pragma warning restore S2436
    : FilteredListWrapper<TItem, TList, TFilteredList>,
        IDisposable
    where TList : IList<TItem>, INotifyCollectionChanged
    where TFilteredList : IList<TItem>
{
    private bool _isUpdating;
    private bool _disposed;

    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="filteredList">过滤列表</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public ObservableFilteredListWrapper(
        TList list,
        TFilteredList filteredList,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
        : base(list, filteredList, filter, autoFilter)
    {
        SourceList.CollectionChanged += OnSourceListCollectionChanged;
    }

    /// <inheritdoc/>
    public override TItem this[int index]
    {
        get => base[index];
        set
        {
            _isUpdating = true;
            try
            {
                base[index] = value;
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }

    /// <inheritdoc/>
    public override void BatchUpdate(Action<TList> updateAction)
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
    public override void Add(TItem item)
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
    public override void Insert(int index, TItem item)
    {
        _isUpdating = true;
        try
        {
            base.Insert(index, item);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override bool Remove(TItem item)
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
    public override void RemoveAt(int index)
    {
        _isUpdating = true;
        try
        {
            base.RemoveAt(index);
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
    /// 释放资源并取消订阅源列表事件
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        SourceList.CollectionChanged -= OnSourceListCollectionChanged;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void OnSourceListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isUpdating)
            return;

        if (e.Action == NotifyCollectionChangedAction.Reset && SourceList.Count == 0)
        {
            FilteredList.Clear();
            return;
        }

        if (AutoFilter is false)
            return;

        Refresh();
    }
}
