using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读过滤列表
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredList"/></para>
/// </summary>
/// <typeparam name="T">项类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ReadOnlyFilterList<T, TFilteredList>
    : IList<T>,
        IReadOnlyList<T>,
        IFilterCollection<T, IObservableList<T>, TFilteredList>,
        IList
    where TFilteredList : IList<T>
{
    private readonly IObservableList<T> _list;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="filteredList">过滤列表</param>
    /// <param name="filter">过滤器</param>
    public ReadOnlyFilterList(
        IObservableList<T> list,
        TFilteredList filteredList,
        Predicate<T> filter
    )
    {
        if (filteredList.IsReadOnly)
            throw new ReadOnlyException("FilteredList is read only");
        _list = list;
        FilteredList = filteredList;
        Filter = filter;
        _list.ListChanged += List_ListChanged;
    }

    private void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
    {
        if (e.Action is ListChangeAction.Add)
        {
            if (Filter(e.NewItem!) is false)
                return;
            FilteredList.Add(e.NewItem!);
            _filteredListIndex.Add(e.Index);
        }
        else if (e.Action is ListChangeAction.Remove)
        {
            if (Filter(e.OldItem!) is false)
                return;
            var index = _filteredListIndex.IndexOf(e.Index);
            if (index < 0)
                return;
            FilteredList.RemoveAt(index);
            _filteredListIndex.RemoveAt(index);
        }
        else if (e.Action is ListChangeAction.Replace)
        {
            var index = _filteredListIndex.IndexOf(e.Index);
            // 如果旧项目存在于过滤列表中
            if (index >= 0)
            {
                // 如果新项目符合过滤条件
                if (Filter(e.NewItem!))
                    FilteredList[index] = e.NewItem!;
                else
                    FilteredList.RemoveAt(index);
            }
            else if (Filter(e.NewItem!))
            {
                // 旧项目不存在但新项目符合过滤条件

                // 获取源索引在索引列表中的位置
                var lIndex = _filteredListIndex.FindLastIndex(i => i < e.Index);
                // 如果索引列表中没有比当前源索引小的索引
                if (lIndex == -1)
                    lIndex = 0;
                FilteredList.Insert(lIndex, e.NewItem!);
                _filteredListIndex.Insert(lIndex, e.Index);
            }
        }
        else if (e.Action is ListChangeAction.Clear)
        {
            FilteredList.Clear();
            _filteredListIndex.Clear();
        }
    }
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
    /// 过滤完成的列表
    /// </summary>
    public TFilteredList FilteredList { get; }

    /// <summary>
    /// 索引表
    /// </summary>
    private List<int> _filteredListIndex = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IObservableList<T> IFilterCollection<T, IObservableList<T>, TFilteredList>.BaseCollection =>
        throw new ReadOnlyException();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredList IFilterCollection<T, IObservableList<T>, TFilteredList>.FilteredCollection =>
        FilteredList;

    /// <summary>
    /// 刷新过滤列表
    /// </summary>
    public void Refresh()
    {
        FilteredList.Clear();
        _filteredListIndex.Clear();
        if (Filter is null)
        {
            for (var i = 0; i < _list.Count; i++)
            {
                FilteredList.Add(_list[i]);
                _filteredListIndex.Add(i);
            }
        }
        else if (_list.HasValue())
        {
            for (var i = 0; i < _list.Count; i++)
            {
                if (Filter(_list[i]) is false)
                    continue;
                FilteredList.Add(_list[i]);
                _filteredListIndex.Add(i);
            }
        }
    }

    #region IList
    /// <inheritdoc/>
    public T this[int index]
    {
        get => ((IList<T>)_list)[index];
        set => throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)_list).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IList)_list).IsFixedSize;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_list).SyncRoot;

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void IList.Clear()
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return ((ICollection<T>)_list).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_list).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_list).GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return ((IList<T>)_list).IndexOf(item);
    }

    /// <inheritdoc/>
    void IList<T>.Insert(int index, T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void IList<T>.RemoveAt(int index)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void IList.RemoveAt(int index)
    {
        throw new ReadOnlyException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
    #endregion

    #region IList
    object? IList.this[int index]
    {
        get => this[index];
        set => throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    int IList.Add(object? value)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>

    bool IList.Contains(object? value)
    {
        return Contains((T)value!);
    }

    /// <inheritdoc/>
    int IList.IndexOf(object? value)
    {
        return ((IList)_list).IndexOf(value);
    }

    /// <inheritdoc/>
    void IList.Insert(int index, object? value)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void IList.Remove(object? value)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }
    #endregion
}
