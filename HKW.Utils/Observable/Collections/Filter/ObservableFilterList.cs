using System.ComponentModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable.Collections;

/// <summary>
/// 可观察过滤列表
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
public class ObservableFilterList<T, TFilteredList>
    : ObservableList<T>,
        IFilterCollection<T, TFilteredList>
    where TFilteredList : IList<T>
{
    #region Ctor

    /// <inheritdoc/>
    public ObservableFilterList()
        : base() { }

    /// <inheritdoc/>
    public ObservableFilterList(int capacity)
        : base(capacity) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableFilterList(IEnumerable<T> collection)
        : base(collection) { }

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
    private TFilteredList _filteredList = default!;

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public required TFilteredList FilteredList
    {
        get => _filteredList;
        init
        {
            _filteredList = value;
            Refresh();
            OnPropertyChanged(nameof(FilteredList));
        }
    }
    TFilteredList IFilterCollection<T, TFilteredList>.FilteredCollection => FilteredList;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (FilteredList is null || Filter is null)
            return;
        FilteredList.Clear();
        if (this.HasValue())
            FilteredList.AddRange(this.Where(i => Filter(i)));
    }
    #endregion

    #region ListChanging
    /// <inheritdoc/>
    protected override void OnListAdded(IList<T> items, int index)
    {
        base.OnListAdded(items, index);
        foreach (var item in items.Where(p => Filter(p)))
            FilteredList.Insert(index++, item);
    }

    /// <inheritdoc/>
    protected override void OnListRemoved(IList<T> items, int index)
    {
        base.OnListRemoved(items, index);
        foreach (var pair in items)
            FilteredList.Remove(pair);
    }

    /// <inheritdoc/>
    protected override void OnListReplaced(IList<T> newItems, IList<T> oldItems, int index)
    {
        base.OnListReplaced(newItems, oldItems, index);
        foreach (var (newIndex, item) in oldItems.EnumerateIndex())
        {
            var filteredListIndex = FilteredList.IndexOf(item);
            if (filteredListIndex != -1)
                FilteredList[filteredListIndex] = newItems[newIndex];
        }
    }

    /// <inheritdoc/>
    protected override void OnListCleared()
    {
        base.OnListCleared();
        FilteredList.Clear();
    }
    #endregion
}
