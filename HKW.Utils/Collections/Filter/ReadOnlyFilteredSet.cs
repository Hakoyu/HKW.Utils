//using System.Collections;
//using System.Diagnostics;
//using HKW.HKWUtils.DebugViews;
//using HKW.HKWUtils.Exceptions;
//using HKW.HKWUtils.Extensions;
//using HKW.HKWUtils.Natives;
//using HKW.HKWUtils.Observable;

//namespace HKW.HKWUtils.Collections;

///// <summary>
///// 只读过滤集合
///// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredSet"/></para>
///// </summary>
//[DebuggerDisplay("Count = {Count}")]
//[DebuggerTypeProxy(typeof(ICollectionDebugView))]
//public class ReadOnlyFilteredSet<T, TFilteredSet>
//    : ISet<T>,
//        IReadOnlySet<T>,
//        IFilteredCollectionWrapper<T, IObservableSet<T>, TFilteredSet>,
//        IDisposable
//    where TFilteredSet : ISet<T>
//{
//    private readonly IObservableSet<T> _set;

//    #region Ctor
//    /// <inheritdoc/>
//    /// <param name="set">集合</param>
//    /// <param name="filteredSet">过滤集合</param>
//    /// <param name="filter">过滤器</param>
//    public ReadOnlyFilteredSet(IObservableSet<T> set, TFilteredSet filteredSet, Predicate<T> filter)
//    {
//        ArgumentNullException.ThrowIfNull(set);
//        ArgumentNullException.ThrowIfNull(filteredSet);
//        ArgumentNullException.ThrowIfNull(filter);
//        ArgumentException.ThrowIfReadOnlyCollection(set);
//        ArgumentException.ThrowIfReadOnlyCollection(filteredSet);

//        _set = set;
//        Filter = filter;
//        FilteredSet = filteredSet;
//        FilteredSet.Clear();
//        foreach (var item in set.Where(x => Filter(x)))
//            FilteredSet.Add(item);

//        set.SetChanged += Set_SetChanged;
//    }

//    private void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
//    {
//        if (e.Action is SetChangeAction.Add)
//        {
//            if (e.NewItems is null)
//                return;
//            foreach (var item in e.NewItems.Where(x => Filter(x)))
//                FilteredSet.Add(item);
//        }
//        else if (e.Action is SetChangeAction.Remove)
//        {
//            if (e.OldItems is null)
//                return;
//            foreach (var item in e.OldItems.Where(x => Filter(x)))
//                FilteredSet.Remove(item);
//        }
//        else if (e.Action is SetChangeAction.Clear)
//        {
//            FilteredSet.Clear();
//        }
//        else if (e.OtherItems is not null)
//        {
//            if (e.OldItems is not null)
//            {
//                foreach (var item in e.OldItems.Where(x => Filter(x)))
//                    FilteredSet.Remove(item);
//            }

//            if (e.NewItems is not null)
//            {
//                foreach (var item in e.NewItems.Where(x => Filter(x)))
//                    FilteredSet.Add(item);
//            }
//        }
//    }
//    #endregion

//    /// <inheritdoc/>
//    bool IFilteredCollectionWrapper<T, IObservableSet<T>, TFilteredSet>.AutoFilter { get; set; } =
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
//    /// 过滤完成的集合
//    /// </summary>
//    public TFilteredSet FilteredSet { get; }

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    IObservableSet<T> ICollectionWrapper<T, IObservableSet<T>>.SourceCollection =>
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    TFilteredSet IFilteredCollectionWrapper<
//        T,
//        IObservableSet<T>,
//        TFilteredSet
//    >.FilteredCollection => FilteredSet;

//    /// <inheritdoc/>
//    public void BatchUpdate(Action<IObservableSet<T>> updateAction)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public void Refresh()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    #region ISet
//    /// <inheritdoc/>
//    public int Count => _set.Count;

//    /// <inheritdoc/>
//    public bool IsReadOnly => true;

//    /// <inheritdoc/>
//    bool ISet<T>.Add(T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ICollection<T>.Clear()
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool Contains(T item)
//    {
//        return _set.Contains(item);
//    }

//    /// <inheritdoc/>
//    public void CopyTo(T[] array, int arrayIndex)
//    {
//        _set.CopyTo(array, arrayIndex);
//    }

//    /// <inheritdoc/>
//    void ISet<T>.ExceptWith(IEnumerable<T> other)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public IEnumerator<T> GetEnumerator()
//    {
//        return _set.GetEnumerator();
//    }

//    /// <inheritdoc/>
//    void ISet<T>.IntersectWith(IEnumerable<T> other)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool IsProperSubsetOf(IEnumerable<T> other)
//    {
//        return _set.IsProperSubsetOf(other);
//    }

//    /// <inheritdoc/>
//    public bool IsProperSupersetOf(IEnumerable<T> other)
//    {
//        return _set.IsProperSupersetOf(other);
//    }

//    /// <inheritdoc/>
//    public bool IsSubsetOf(IEnumerable<T> other)
//    {
//        return _set.IsSubsetOf(other);
//    }

//    /// <inheritdoc/>
//    public bool IsSupersetOf(IEnumerable<T> other)
//    {
//        return _set.IsSupersetOf(other);
//    }

//    /// <inheritdoc/>
//    public bool Overlaps(IEnumerable<T> other)
//    {
//        return _set.Overlaps(other);
//    }

//    /// <inheritdoc/>
//    bool ICollection<T>.Remove(T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    public bool SetEquals(IEnumerable<T> other)
//    {
//        return _set.SetEquals(other);
//    }

//    /// <inheritdoc/>
//    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    /// <inheritdoc/>
//    void ISet<T>.UnionWith(IEnumerable<T> other)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    void ICollection<T>.Add(T item)
//    {
//        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return ((IEnumerable)_set).GetEnumerator();
//    }
//    #endregion

//    #region IDisposable
//    private bool _disposed;

//    /// <inheritdoc/>
//    ~ReadOnlyFilteredSet()
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
//            _set.SetChanged -= Set_SetChanged;
//        }

//        _disposed = true;
//    }
//    #endregion
//}
