using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.DebugViews;
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
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class FilterListWrapper<TItem, TList, TFilteredList>
    : IList<TItem>,
        IReadOnlyList<TItem>,
        IFilterCollection<TItem, TList, TFilteredList>,
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
    public FilterListWrapper(TList list, TFilteredList filteredList, Predicate<TItem> filter)
    {
        if (filteredList.IsReadOnly)
            throw new ReadOnlyException("FilteredList is read only");
        BaseList = list;
        FilteredList = filteredList;
        Filter = filter;
    }

    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="getFilteredList">获取过滤列表</param>
    /// <param name="filter">过滤器</param>
    public FilterListWrapper(
        TList list,
        Func<TList, TFilteredList> getFilteredList,
        Predicate<TItem> filter
    )
        : this(list, getFilteredList(list), filter) { }
    #endregion

    /// <inheritdoc/>
    public bool AutoFilter { get; set; } = true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<TItem> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <inheritdoc/>
    public TList BaseList { get; }

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public TFilteredList FilteredList { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TList IFilterCollection<TItem, TList, TFilteredList>.BaseCollection => BaseList;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredList IFilterCollection<TItem, TList, TFilteredList>.FilteredCollection => FilteredList;

    /// <summary>
    /// 刷新过滤列表
    /// </summary>
    public void Refresh()
    {
        FilteredList.Clear();
        if (Filter is null)
            FilteredList.AddRange(BaseList);
        else if (BaseList.HasValue())
            FilteredList.AddRange(BaseList.Where(i => Filter(i)));
    }

    #region IList
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => ((IList<TItem>)BaseList)[index];
        set
        {
            var oldValue = BaseList[index];
            ((IList<TItem>)BaseList)[index] = value;
            if (AutoFilter is false || FilteredList.IsReadOnly)
                return;
            if (Filter(value) is false)
                return;
            var tempIndex = FilteredList.IndexOf(oldValue);
            if (tempIndex != -1)
                FilteredList[tempIndex] = value;
            else
                Refresh();
        }
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)BaseList).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)BaseList).IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)BaseList).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)BaseList).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)BaseList).SyncRoot;

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        ((ICollection<TItem>)BaseList).Add(item);
        if (AutoFilter is false || FilteredList.IsReadOnly)
            return;
        if (Filter(item))
            FilteredList.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)BaseList).Clear();
        if (AutoFilter is false || FilteredList.IsReadOnly)
            return;
        FilteredList.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)BaseList).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)BaseList).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)BaseList).GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return ((IList<TItem>)BaseList).IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        ((IList<TItem>)BaseList).Insert(index, item);
        if (AutoFilter is false || FilteredList.IsReadOnly)
            return;
        if (Filter(item))
        {
            if (FilteredList.HasValue() is false)
                FilteredList.Add(item);
            else
            {
                var set = BaseList.Take(index).ToHashSet();
                var last1 = FilteredList.FindLastIndex(x => set.Contains(x));
                if (last1 == -1)
                    FilteredList.Insert(0, item);
                else
                {
                    var last2 = FilteredList.FindLastIndex(x =>
                        x?.Equals(FilteredList[last1]) is true
                    );
                    FilteredList.Insert(last2 + 1, item);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = ((ICollection<TItem>)BaseList).Remove(item);
        if (AutoFilter is false || FilteredList.IsReadOnly)
            return result;
        if (result)
            FilteredList.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        if (BaseList.TryGetValue(index, out var value) is false)
            return;
        BaseList.RemoveAt(index);
        if (AutoFilter is false || FilteredList.IsReadOnly)
            return;
        FilteredList.Remove(value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseList).GetEnumerator();
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
        return ((IList)BaseList).IndexOf(value);
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
        ((ICollection)BaseList).CopyTo(array, index);
    }
    #endregion
}
