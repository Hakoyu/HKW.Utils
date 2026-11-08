using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察的过滤集合
/// <para>基于 <see cref="FilteredSetWrapper{TItem, TSet, TFilteredSet}"/> 维护一个实时过滤的集合</para>
/// <para>源集合发生变更时, 会自动同步更新 <see cref="FilteredSetWrapper{TItem, TSet, TFilteredSet}.FilteredSet"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
/// <typeparam name="TFilteredSet">已过滤集合类型</typeparam>
#pragma warning disable S2436
public sealed class ObservableFilteredSetWrapper<TItem, TSet, TFilteredSet>
#pragma warning restore S2436
    : FilteredSetWrapper<TItem, TSet, TFilteredSet>,
        IDisposable
    where TSet : ISet<TItem>, INotifyCollectionChanged
    where TFilteredSet : ISet<TItem>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="filteredSet">过滤集合</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public ObservableFilteredSetWrapper(
        TSet set,
        TFilteredSet filteredSet,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
        : base(set, filteredSet, filter, autoFilter)
    {
        _subscribedSource = SourceSet;
        _subscribedSource.CollectionChanged += OnSourceSetCollectionChanged;
    }

    private TSet? _subscribedSource;

    /// <inheritdoc/>
    protected override void OnSourceChanged(TSet previous)
    {
        previous.CollectionChanged -= OnSourceSetCollectionChanged;
        _subscribedSource = SourceSet;
        _subscribedSource.CollectionChanged += OnSourceSetCollectionChanged;
    }

    /// <summary>
    /// 释放资源并取消订阅源集合事件
    /// </summary>
    public void Dispose()
    {
        DisposeFilter(() =>
        {
            if (_subscribedSource is not null)
                _subscribedSource.CollectionChanged -= OnSourceSetCollectionChanged;
            _subscribedSource = default;
        });
        GC.SuppressFinalize(this);
    }

    private void OnSourceSetCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (ReferenceEquals(sender, _subscribedSource) is false)
            return;
        SourceChanged(e.Action == NotifyCollectionChangedAction.Reset && SourceSet.Count == 0);
    }
}
