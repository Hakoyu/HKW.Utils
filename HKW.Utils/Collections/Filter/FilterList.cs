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
/// <typeparam name="T">项目类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class FilterList<T, TList, TFilteredList>
    : IList<T>,
        IReadOnlyList<T>,
        IFilterCollection<T, TList, TFilteredList>,
        IList
    where TList : IList<T>
    where TFilteredList : IList<T>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="filteredList">过滤列表</param>
    /// <param name="filter">过滤器</param>
    public FilterList(TList list, TFilteredList filteredList, Predicate<T> filter)
    {
        if (filteredList.IsReadOnly)
            throw new ReadOnlyException("FilteredList is read only");
        List = list;
        FilteredList = filteredList;
        Filter = filter;
    }

    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="getFilteredList">获取过滤列表</param>
    /// <param name="filter">过滤器</param>
    public FilterList(TList list, Func<TList, TFilteredList> getFilteredList, Predicate<T> filter)
        : this(list, getFilteredList(list), filter) { }
    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<T> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<T> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <summary>
    /// 列表
    /// <para>使用此属性修改列表时不会同步至 <see cref="FilteredList"/></para>
    /// </summary>
    public TList List { get; }

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public TFilteredList FilteredList { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TList IFilterCollection<T, TList, TFilteredList>.Collection => List;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredList IFilterCollection<T, TList, TFilteredList>.FilteredCollection => FilteredList;

    /// <summary>
    /// 刷新过滤列表
    /// </summary>
    public void Refresh()
    {
        FilteredList.Clear();
        if (Filter is null)
            FilteredList.AddRange(List);
        else if (List.HasValue())
            FilteredList.AddRange(List.Where(i => Filter(i)));
    }

    #region IList
    /// <inheritdoc/>
    public T this[int index]
    {
        get => ((IList<T>)List)[index];
        set
        {
            var oldValue = List[index];
            ((IList<T>)List)[index] = value;
            if (Filter(value) is false || FilteredList.IsReadOnly)
                return;
            var tempIndex = FilteredList.IndexOf(oldValue);
            if (tempIndex != -1)
                FilteredList[tempIndex] = value;
            else
                Refresh();
        }
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)List).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)List).IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)List).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)List).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)List).SyncRoot;

    /// <inheritdoc/>
    public void Add(T item)
    {
        ((ICollection<T>)List).Add(item);
        if (FilteredList.IsReadOnly)
            return;
        if (Filter(item))
            FilteredList.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)List).Clear();
        if (FilteredList.IsReadOnly)
            return;
        FilteredList.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)List).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)List).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)List).GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return ((IList<T>)List).IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        ((IList<T>)List).Insert(index, item);
        if (FilteredList.IsReadOnly)
            return;
        if (Filter(item))
            FilteredList.Insert(index, item);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var result = ((ICollection<T>)List).Remove(item);
        if (FilteredList.IsReadOnly)
            return result;
        if (result)
            FilteredList.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        if (List.TryGetValue(index, out var value) is false)
            return;
        List.RemoveAt(index);
        if (FilteredList.IsReadOnly)
            return;
        FilteredList.Remove(value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)List).GetEnumerator();
    }
    #endregion

    #region IList
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <inheritdoc/>
    int IList.Add(object? value)
    {
        Add((T)value!);
        return Count - 1;
    }

    /// <inheritdoc/>

    bool IList.Contains(object? value)
    {
        return Contains((T)value!);
    }

    /// <inheritdoc/>
    int IList.IndexOf(object? value)
    {
        return ((IList)List).IndexOf(value);
    }

    /// <inheritdoc/>
    void IList.Insert(int index, object? value)
    {
        Insert(index, (T)value!);
    }

    /// <inheritdoc/>
    void IList.Remove(object? value)
    {
        Remove((T)value!);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)List).CopyTo(array, index);
    }
    #endregion
}
