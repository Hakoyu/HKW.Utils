//using System.Collections;
//using System.Diagnostics;
//using HKW.HKWUtils.DebugViews;
//using HKW.HKWUtils.Exceptions;
//using HKW.HKWUtils.Extensions;
//using HKW.HKWUtils.Natives;
//using HKW.HKWUtils.Observable;

//namespace HKW.HKWUtils.Collections;

///// <summary>
///// 只读过滤列表
///// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredList"/></para>
///// </summary>
///// <typeparam name="T">项类型</typeparam>
///// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
//[DebuggerDisplay("Count = {Count}")]
//[DebuggerTypeProxy(typeof(ICollectionDebugView))]
//public class ReadOnlyFilteredList<T, TFilteredList>
//    : IList<T>,
//        IReadOnlyList<T>,
//        IFilteredCollectionWrapper<T, IObservableList<T>, TFilteredList>,
//        IList,
//        IDisposable
//    where TFilteredList : IList<T>
//{
//    private readonly IObservableList<T> _list;

//    #region Ctor
//    /// <inheritdoc/>
//    /// <param name="list">列表</param>
//    /// <param name="filteredList">过滤列表</param>
//    /// <param name="filter">过滤器</param>
//    public ReadOnlyFilteredList(
//        IObservableList<T> list,
//        TFilteredList filteredList,
//        Predicate<T> filter
//    )
//    {
//        ArgumentNullException.ThrowIfNull(list);
//        ArgumentNullException.ThrowIfNull(filteredList);
//        ArgumentNullException.ThrowIfNull(filter);
//        ArgumentException.ThrowIfReadOnlyCollection(list);
//        ArgumentException.ThrowIfReadOnlyCollection(filteredList);

//        _list = list;
//        Filter = filter;
//        FilteredList = filteredList;
//        FilteredList.Clear();
//        foreach (var (e, i) in list.Where(x => Filter(x)).WithIndex())
//        {
//            FilteredList.Add(e);
//            _filteredListIndex.Add(i);
//        }

//        _list.ListChanged += List_ListChanged;
//    }

//    private void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
//    {
//        if (e.Action is ListChangeAction.Add)
//        {
//            if (Filter(e.NewItem!) is false)
//                return;
//            FilteredList.Add(e.NewItem!);
//            _filteredListIndex.Add(e.Index);
//        }
//        else if (e.Action is ListChangeAction.Remove)
//        {
//            if (Filter(e.OldItem!) is false)
//                return;
//            var index = _filteredListIndex.IndexOf(e.Index);
//            if (index < 0)
//                return;
//            FilteredList.RemoveAt(index);
//            _filteredListIndex.RemoveAt(index);
//        }
//        else if (e.Action is ListChangeAction.Replace)
//        {
//            var index = _filteredListIndex.IndexOf(e.Index);
//            // 如果旧项目存在于过滤列表中
//            if (index >= 0)
//            {
//                // 如果新项目符合过滤条件
//                if (Filter(e.NewItem!))
//                    FilteredList[index] = e.NewItem!;
//                else
//                    FilteredList.RemoveAt(index);
//            }
//            else if (Filter(e.NewItem!))
//            {
//                // 旧项目不存在但新项目符合过滤条件

//                // 获取源索引在索引列表中的位置
//                var lIndex = _filteredListIndex.FindIndex(i => i > e.Index);
//                // 如果索引列表中没有比当前源索引小的索引
//                if (lIndex == -1)
//                    lIndex = FilteredList.Count;
//                FilteredList.Insert(lIndex, e.NewItem!);
//                _filteredListIndex.Insert(lIndex, e.Index);
//            }
//        }
//        else if (e.Action is ListChangeAction.Clear)
//        {
//            FilteredList.Clear();
//            _filteredListIndex.Clear();
//        }
//    }
//    #endregion
//    /// <inheritdoc/>
//    bool IFilteredCollectionWrapper<T, IObservableList<T>, TFilteredList>.AutoFilter { get; set; } =
//        true;

//    /// <summary>
//    /// 过滤器
//    /// </summary>
//    public Predicate<T> Filter
//    {
//        get => field;
//        set =>
//            field = field is null
//                ? value
//                : throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <summary>
//    /// 过滤完成的列表
//    /// </summary>
//    public TFilteredList FilteredList { get; }

//    /// <summary>
//    /// 索引表
//    /// </summary>
//    private readonly List<int> _filteredListIndex = [];

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    IObservableList<T> ICollectionWrapper<T, IObservableList<T>>.SourceCollection =>
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    TFilteredList IFilteredCollectionWrapper<
//        T,
//        IObservableList<T>,
//        TFilteredList
//    >.FilteredCollection => FilteredList;

//    /// <inheritdoc/>
//    public void BatchUpdate(Action<IObservableList<T>> updateAction)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public void Refresh()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    #region IList
//    /// <inheritdoc/>
//    public T this[int index]
//    {
//        get => _list[index];
//        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public int Count => _list.Count;

//    /// <inheritdoc/>
//    public bool IsReadOnly => true;

//    /// <inheritdoc/>
//    public bool IsFixedSize => ((IList)_list).IsFixedSize;

//    /// <inheritdoc/>
//    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

//    /// <inheritdoc/>
//    public object SyncRoot => ((ICollection)_list).SyncRoot;

//    /// <inheritdoc/>
//    void ICollection<T>.Add(T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ICollection<T>.Clear()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IList.Clear()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool Contains(T item)
//    {
//        return _list.Contains(item);
//    }

//    /// <inheritdoc/>
//    public void CopyTo(T[] array, int arrayIndex)
//    {
//        _list.CopyTo(array, arrayIndex);
//    }

//    /// <inheritdoc/>
//    public IEnumerator<T> GetEnumerator()
//    {
//        return _list.GetEnumerator();
//    }

//    /// <inheritdoc/>
//    public int IndexOf(T item)
//    {
//        return _list.IndexOf(item);
//    }

//    /// <inheritdoc/>
//    void IList<T>.Insert(int index, T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    bool ICollection<T>.Remove(T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IList<T>.RemoveAt(int index)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IList.RemoveAt(int index)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return ((IEnumerable)_list).GetEnumerator();
//    }
//    #endregion

//    #region IList
//    object? IList.this[int index]
//    {
//        get => this[index];
//        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    int IList.Add(object? value)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    bool IList.Contains(object? value)
//    {
//        return Contains((T)value!);
//    }

//    /// <inheritdoc/>
//    int IList.IndexOf(object? value)
//    {
//        return ((IList)_list).IndexOf(value);
//    }

//    /// <inheritdoc/>
//    void IList.Insert(int index, object? value)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void IList.Remove(object? value)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ICollection.CopyTo(Array array, int index)
//    {
//        ((ICollection)_list).CopyTo(array, index);
//    }
//    #endregion

//    #region IDisposable
//    private bool _disposed;

//    /// <inheritdoc/>
//    ~ReadOnlyFilteredList()
//    {
//        Dispose(false);
//    }

//    /// <inheritdoc/>
//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    /// <inheritdoc/>
//    protected virtual void Dispose(bool disposing)
//    {
//        if (_disposed)
//            return;

//        if (disposing)
//        {
//            _list.ListChanged -= List_ListChanged;
//        }

//        _disposed = true;
//    }
//    #endregion
//}
