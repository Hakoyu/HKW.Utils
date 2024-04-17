using System;
using System.Collections;
using System.Collections.Generic;
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
/// <typeparam name="TItem">项目类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class FilterList<TItem, TFilteredList>
    : IListRange<TItem>,
        IReadOnlyList<TItem>,
        IFilterCollection<TItem, TFilteredList>,
        IList
    where TFilteredList : IList<TItem>
{
    private readonly List<TItem> _list;

    private Predicate<TItem> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public required Predicate<TItem> Filter
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
        }
    }

    TFilteredList IFilterCollection<TItem, TFilteredList>.FilteredCollection => FilteredList;

    #region Ctor
    /// <inheritdoc/>
    public FilterList()
        : this(null, null) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    public FilterList(int capacity)
        : this(capacity, null) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public FilterList(IEnumerable<TItem> collection)
        : this(null, collection) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="collection">集合</param>
    private FilterList(int? capacity, IEnumerable<TItem>? collection)
    {
        if (capacity is not null)
            _list = new(capacity.Value);
        else if (collection is not null)
            _list = new(collection);
        else
            _list = new();
    }
    #endregion

    /// <summary>
    /// 刷新过滤列表
    /// </summary>
    public void Refresh()
    {
        if (FilteredList is null || Filter is null)
            return;
        FilteredList.Clear();
        if (_list.HasValue())
            FilteredList.AddRange(_list.Where(i => Filter(i)));
    }

    #region IList
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => ((IList<TItem>)_list)[index];
        set
        {
            var oldValue = _list[index];
            ((IList<TItem>)_list)[index] = value;
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
    public int Count => ((ICollection<TItem>)_list).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)_list).IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)_list).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_list).SyncRoot;

    object? IList.this[int index]
    {
        get => ((IList)_list)[index];
        set => ((IList)_list)[index] = value;
    }

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        ((ICollection<TItem>)_list).Add(item);
        if (Filter(item))
            FilteredList.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)_list).Clear();
        FilteredList.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return ((ICollection<TItem>)_list).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        ((ICollection<TItem>)_list).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)_list).GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return ((IList<TItem>)_list).IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        ((IList<TItem>)_list).Insert(index, item);
        if (Filter(item))
            FilteredList.Insert(index, item);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = ((ICollection<TItem>)_list).Remove(item);
        if (result)
            FilteredList.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        if (_list.TryGetValue(index, out var item) is false)
            return;
        _list.RemoveAt(index);
        FilteredList.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
    #endregion

    #region IListX
    /// <inheritdoc/>
    public void AddRange(IEnumerable<TItem> collection)
    {
        _list.AddRange(collection);
        FilteredList.AddRange(collection.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<TItem> collection)
    {
        _list.InsertRange(index, collection);
        FilteredList.InsertRange(index, collection.Where(i => Filter(i)));
    }

    /// <inheritdoc/>
    public void RemoveAll(Predicate<TItem> match)
    {
        _list.RemoveAll(match);
        FilteredList.RemoveAll(match);
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        _list.RemoveRange(index, count);
        FilteredList.RemoveRange(index, count);
    }

    /// <inheritdoc/>
    public void Reverse()
    {
        _list.Reverse();
        FilteredList.Reverse();
    }

    /// <inheritdoc/>
    public void Reverse(int index, int count)
    {
        _list.Reverse(index, count);
        FilteredList.Reverse(index, count);
    }
    #endregion

    #region IList
    /// <inheritdoc/>
    public int Add(object? value)
    {
        var result = ((IList)_list).Add(value);
        if (result != -1)
            FilteredList.Add((TItem)value!);
        return result;
    }

    /// <inheritdoc/>

    public bool Contains(object? value)
    {
        return ((IList)_list).Contains(value);
    }

    /// <inheritdoc/>
    public int IndexOf(object? value)
    {
        return ((IList)_list).IndexOf(value);
    }

    /// <inheritdoc/>
    public void Insert(int index, object? value)
    {
        ((IList)_list).Insert(index, value);
        if (Filter((TItem)value!))
            FilteredList.Insert(index, (TItem)value!);
    }

    /// <inheritdoc/>
    public void Remove(object? value)
    {
        var count = Count;
        ((IList)_list).Remove(value);
        if (count != Count)
            FilteredList.Remove((TItem)value!);
    }

    /// <inheritdoc/>
    public void CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }
    #endregion
}
