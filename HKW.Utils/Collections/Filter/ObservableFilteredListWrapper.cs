using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察的过滤列表
/// <para>基于 <see cref="FilteredListWrapper{TItem, TList, TFilteredList}"/> 维护一个实时过滤的列表</para>
/// <para>源列表发生变更时, 会自动同步更新 <see cref="FilteredListWrapper{TItem, TList, TFilteredList}.FilteredList"/></para>
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
    private TList? _subscribedSource;

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
        _subscribedSource = SourceList;
        _subscribedSource.CollectionChanged += OnSourceListCollectionChanged;
    }

    /// <inheritdoc/>
    protected override void OnSourceChanged(TList previous)
    {
        if (_subscribedSource is not null)
            _subscribedSource.CollectionChanged -= OnSourceListCollectionChanged;
        _subscribedSource = SourceList;
        _subscribedSource.CollectionChanged += OnSourceListCollectionChanged;
    }

    /// <summary>
    /// 释放资源并取消订阅源列表事件
    /// </summary>
    public void Dispose()
    {
        DisposeFilter(() =>
        {
            if (_subscribedSource is not null)
                _subscribedSource.CollectionChanged -= OnSourceListCollectionChanged;
            _subscribedSource = default;
        });
        GC.SuppressFinalize(this);
    }

    private void OnSourceListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (ReferenceEquals(sender, _subscribedSource) is false)
            return;
        SourceChanged(e.Action == NotifyCollectionChangedAction.Reset && SourceList.Count == 0);
    }
}
