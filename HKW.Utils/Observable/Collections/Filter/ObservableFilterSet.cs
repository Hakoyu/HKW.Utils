using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable.Collections;

/// <summary>
/// 可观察过滤集合
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
/// <typeparam name="TFilteredSet">过滤集合类型</typeparam>
public class ObservableFilterSet<T, TFilteredSet>
    : ObservableSet<T>,
        IFilterCollection<T, TFilteredSet>
    where TFilteredSet : ISet<T>
{
    #region Ctor

    /// <inheritdoc/>
    public ObservableFilterSet()
        : base() { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public ObservableFilterSet(int capacity)
        : base(capacity) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableFilterSet(IEqualityComparer<T> comparer)
        : base(comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableFilterSet(IEnumerable<T> collection)
        : base(collection) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableFilterSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
        : base(collection, comparer) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableFilterSet(int capacity, IEqualityComparer<T>? comparer)
        : base(capacity, comparer) { }
    #endregion
    #region IFilterCollection

    private Predicate<T> _filter = null!;

    /// <inheritdoc/>
    public required Predicate<T> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }
    private TFilteredSet _filteredSet = default!;

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public required TFilteredSet FilteredSet
    {
        get => _filteredSet;
        init
        {
            _filteredSet = value;
            Refresh();
            OnPropertyChanged(nameof(FilteredSet));
        }
    }
    TFilteredSet IFilterCollection<T, TFilteredSet>.FilteredCollection => FilteredSet;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (FilteredSet is null || Filter is null)
            return;
        FilteredSet.Clear();
        if (this.HasValue())
            FilteredSet.IntersectWith(this.Where(i => Filter(i)));
    }
    #endregion

    #region ListChanged
    /// <inheritdoc/>
    protected override void OnSetAdded(IList<T> items)
    {
        base.OnSetAdded(items);
        FilteredSet.AddRange(items.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    protected override void OnSetRemoved(IList<T> items)
    {
        base.OnSetRemoved(items);
        foreach (var item in items)
            FilteredSet.Remove(item);
    }

    /// <inheritdoc/>
    protected override void OnSetCleared()
    {
        base.OnSetCleared();
        FilteredSet.Clear();
    }

    /// <inheritdoc/>
    protected override void OnSetOperated(
        SetChangeAction action,
        IList<T> otherItems,
        IList<T>? newItems,
        IList<T>? oldTiems
    )
    {
        base.OnSetOperated(action, otherItems, newItems, oldTiems);
        if (action is SetChangeAction.Intersect)
            FilteredSet.IntersectWith(otherItems.Where(i => Filter(i)));
        else if (action is SetChangeAction.Except)
            FilteredSet.ExceptWith(otherItems.Where(i => Filter(i)));
        else if (action is SetChangeAction.SymmetricExcept)
            FilteredSet.SymmetricExceptWith(otherItems.Where(i => Filter(i)));
        else if (action is SetChangeAction.Union)
            FilteredSet.Union(otherItems.Where(i => Filter(i)));
    }
    #endregion
}
