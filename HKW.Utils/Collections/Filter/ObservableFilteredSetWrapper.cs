using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察的过滤集合
/// <para>基于 <see cref="FilteredSetWrapper{TItem, TSet, TFilteredSet}"/> 维护一个实时过滤的集合</para>
/// <para>源集合发生变更时，会自动同步更新 <see cref="FilteredSetWrapper{TItem, TSet, TFilteredSet}.FilteredSet"/></para>
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
    private bool _isUpdating;
    private bool _disposed;

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
        SourceSet.CollectionChanged += OnSourceSetCollectionChanged;
    }

    /// <inheritdoc/>
    public override void BatchUpdate(Action<TSet> updateAction)
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
    public override bool Add(TItem item)
    {
        _isUpdating = true;
        try
        {
            return base.Add(item);
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

    /// <inheritdoc/>
    public override void ExceptWith(IEnumerable<TItem> other)
    {
        _isUpdating = true;
        try
        {
            base.ExceptWith(other);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override void IntersectWith(IEnumerable<TItem> other)
    {
        _isUpdating = true;
        try
        {
            base.IntersectWith(other);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override void SymmetricExceptWith(IEnumerable<TItem> other)
    {
        _isUpdating = true;
        try
        {
            base.SymmetricExceptWith(other);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <inheritdoc/>
    public override void UnionWith(IEnumerable<TItem> other)
    {
        _isUpdating = true;
        try
        {
            base.UnionWith(other);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <summary>
    /// 释放资源并取消订阅源集合事件
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        SourceSet.CollectionChanged -= OnSourceSetCollectionChanged;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void OnSourceSetCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isUpdating)
            return;

        if (e.Action == NotifyCollectionChangedAction.Reset && SourceSet.Count == 0)
        {
            FilteredSet.Clear();
            return;
        }

        if (AutoFilter is false)
            return;

        Refresh();
    }
}
