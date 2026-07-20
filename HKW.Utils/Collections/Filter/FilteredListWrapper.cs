using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤列表
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredList"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredListWrapper<TItem, TList, TFilteredList>
#pragma warning restore S2436
    : IList<TItem>,
        IReadOnlyList<TItem>,
        IFilteredCollectionWrapper<TItem, TList, TFilteredList>,
        IList,
        IListWrapper<TItem, TList>
    where TList : IList<TItem>
    where TFilteredList : IList<TItem>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="filteredList">过滤列表</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredListWrapper(
        TList list,
        TFilteredList filteredList,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(filteredList);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(list);
        ArgumentException.ThrowIfReadOnlyCollection(filteredList);

        SourceList = list;
        FilteredList = filteredList;
        Filter = filter;
        AutoFilter = autoFilter;
        Refresh();
    }
    #endregion

    /// <inheritdoc/>
    public bool AutoFilter { get; set; }

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => field;
        set
        {
            field = value;
            Refresh();
        }
    }

    /// <inheritdoc/>
    public TList SourceList { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TList ICollectionWrapper<TItem, TList>.SourceCollection => SourceList;

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public TFilteredList FilteredList { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredList IFilteredCollectionWrapper<TItem, TList, TFilteredList>.FilteredCollection =>
        FilteredList;

    /// <inheritdoc/>
    public void BatchUpdate(Action<TList> updateAction)
    {
        ArgumentNullException.ThrowIfNull(updateAction);

        var autoFilter = AutoFilter;
        AutoFilter = false;
        try
        {
            updateAction(SourceList);
        }
        finally
        {
            AutoFilter = autoFilter;
        }

        Refresh();
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        FilteredList.Clear();
        if (SourceList.Count > 0)
            FilteredList.AddRange(SourceList.Where(i => Filter(i)));
    }

    #region IList
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => SourceList[index];
        set
        {
            var oldValue = SourceList[index];
            SourceList[index] = value;
            if (AutoFilter is false)
                return;

            var oldIndex = FilteredList.IndexOf(oldValue);
            if (Filter(value))
            {
                if (oldIndex >= 0)
                    FilteredList[oldIndex] = value;
                else
                    Refresh();
            }
            else if (oldIndex >= 0)
            {
                FilteredList.RemoveAt(oldIndex);
            }
        }
    }

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)SourceList).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)SourceList).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)SourceList).SyncRoot;

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        SourceList.Add(item);
        if (AutoFilter is false)
            return;
        if (Filter(item))
            FilteredList.Add(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        SourceList.Insert(index, item);
        if (AutoFilter is false)
            return;
        if (Filter(item))
            Refresh();
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = SourceList.Remove(item);
        if (result)
            FilteredList.Remove(item);
        if (AutoFilter is false)
            return result;
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        var value = SourceList[index];
        SourceList.RemoveAt(index);
        if (AutoFilter is false)
            return;
        FilteredList.Remove(value);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceList.Clear();
        FilteredList.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }
    #endregion

    #region IList
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (TItem)value!;
    }

    /// <inheritdoc/>
    int IList.Add(object? value)
    {
        Add((TItem)value!);
        return Count - 1;
    }

    /// <inheritdoc/>
    bool IList.Contains(object? value)
    {
        return Contains((TItem)value!);
    }

    /// <inheritdoc/>
    int IList.IndexOf(object? value)
    {
        return ((IList)SourceList).IndexOf(value);
    }

    /// <inheritdoc/>
    void IList.Insert(int index, object? value)
    {
        Insert(index, (TItem)value!);
    }

    /// <inheritdoc/>
    void IList.Remove(object? value)
    {
        Remove((TItem)value!);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)SourceList).CopyTo(array, index);
    }
    #endregion
}
