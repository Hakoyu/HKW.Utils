using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableList<T> : IObservableList<T>, IObservableList, IReadOnlyObservableList<T>
{
    /// <summary>
    /// 原始列表
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly List<T> r_list;

    #region Ctor

    /// <inheritdoc/>
    public ObservableList()
    {
        r_list = new();
    }

    /// <inheritdoc/>
    public ObservableList(int capacity)
    {
        r_list = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableList(IEnumerable<T> collection)
    {
        r_list = new(collection);
    }

    #endregion Ctor

    #region IListT

    /// <inheritdoc/>
    public int Count => r_list.Count;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<T>)r_list).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public T this[int index]
    {
        get => r_list[index];
        set
        {
            var oldValue = r_list[index];
            if (oldValue?.Equals(value) is true || OnListValueChanging(value, oldValue, index))
                return;
            r_list[index] = value;
            OnListValueChanged(value, oldValue, index);
        }
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        if (OnListAdding(item))
            return;
        r_list.Add(item);
        OnListAdded(item);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        if (OnListAdding(item, index))
            return;
        r_list.Insert(index, item);
        OnListAdded(item, index);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (OnListRemoving(item))
            return false;
        var index = r_list.IndexOf(item);
        if (index >= 0)
        {
            r_list.RemoveAt(index);
            OnListRemoved(item, index);
            OnCountPropertyChanged();
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        var item = r_list[index];
        if (OnListRemoving(item, index))
            return;
        r_list.RemoveAt(index);
        OnListRemoved(item, index);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var oldCount = Count;
        if (OnListClearing())
            return;
        r_list.Clear();
        OnListCleared();
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return r_list.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        r_list.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return r_list.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return r_list.IndexOf(item);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_list).GetEnumerator();
    }

    #endregion IListT

    #region IList

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsFixedSize => ((IList)r_list).IsFixedSize;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => ((IList)r_list).IsSynchronized;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => ((IList)r_list).SyncRoot;

    object? IList.this[int index]
    {
        get => r_list[index];
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            r_list[index] = (T)value;
        }
    }

    int IList.Add(object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        Add((T)value);
        return Count - 1;
    }

    bool IList.Contains(object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        return Contains((T)value);
    }

    int IList.IndexOf(object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        return IndexOf((T)value);
    }

    void IList.Insert(int index, object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        Insert(index, (T)value);
    }

    void IList.Remove(object? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        Remove((T)value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        r_list.CopyTo((T[])array, index);
    }

    #endregion IList

    #region IObservableList

    event XCancelEventHandler<NotifyListChangingEventArgs<object>>? INotifyListChanging.ListChanging
    {
        add
        {
            ListChanging += (v) =>
            {
                INotifyListChangingAction(v, value);
            };
        }
        remove
        {
            ListChanging -= (v) =>
            {
                INotifyListChangingAction(v, value);
            };
        }
    }

    event XEventHandler<NotifyListChangedEventArgs<object>>? INotifyListChanged.ListChanged
    {
        add
        {
            ListChanged += (v) =>
            {
                INotifyListChangedAction(v, value);
            };
        }
        remove
        {
            ListChanged -= (v) =>
            {
                INotifyListChangedAction(v, value);
            };
        }
    }

    private static void INotifyListChangingAction(
        NotifyListChangingEventArgs<T> args,
        XCancelEventHandler<NotifyListChangingEventArgs<object>>? nonGenericEvent
    )
    {
        if (nonGenericEvent is null)
            return;
        NotifyListChangingEventArgs<object> newArgs;
        if (args.Action is ListChangeAction.Clear)
            newArgs = new(args.Action);
        else if (args.Action is ListChangeAction.Add)
            newArgs = new(args.Action, args.NewItems!, args.Index);
        else if (args.Action is ListChangeAction.Remove)
            newArgs = new(args.Action, args.OldItems!, args.Index);
        else
            newArgs = new(args.Action, args.NewItems!, args.OldItems!, args.Index);
        nonGenericEvent?.Invoke(newArgs);
        args.Cancel = newArgs.Cancel;
    }

    private static void INotifyListChangedAction(
        NotifyListChangedEventArgs<T> args,
        XEventHandler<NotifyListChangedEventArgs<object>>? nonGenericEvent
    )
    {
        if (nonGenericEvent is null)
            return;
        NotifyListChangedEventArgs<object> newArgs;
        if (args.Action is ListChangeAction.Clear)
            newArgs = new(args.Action);
        else if (args.Action is ListChangeAction.Add)
            newArgs = new(args.Action, args.NewItems!, args.Index);
        else if (args.Action is ListChangeAction.Remove)
            newArgs = new(args.Action, args.OldItems!, args.Index);
        else
            newArgs = new(args.Action, args.NewItems!, args.OldItems!, args.Index);
        nonGenericEvent?.Invoke(newArgs);
    }

    #endregion IObservableList

    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListAdding(T item, int? index = null)
    {
        return OnListChanging(new(ListChangeAction.Add, item, index ?? Count));
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListRemoving(T item, int? index = null)
    {
        return OnListChanging(new(ListChangeAction.Remove, item, index ?? Count - 1));
    }

    /// <summary>
    /// 列表清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListClearing()
    {
        return OnListChanging(new(ListChangeAction.Clear));
    }

    /// <summary>
    /// 列表改变项目前
    /// </summary>
    /// <param name="newValue">新项目</param>
    /// <param name="oldValue">旧项目</param>
    /// <param name="index"></param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListValueChanging(T newValue, T oldValue, int index)
    {
        return OnListChanging(new(ListChangeAction.ValueChange, newValue, oldValue, index));
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual bool OnListChanging(NotifyListChangingEventArgs<T> args)
    {
        ListChanging?.Invoke(args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<NotifyListChangingEventArgs<T>>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    private void OnListAdded(T item, int? index = null)
    {
        var currentIndex = index ?? Count - 1;
        OnListChanged(new(ListChangeAction.Add, item, currentIndex));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index: currentIndex));
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    private void OnListRemoved(T item, int? index = null)
    {
        var currentIndex = index ?? Count;
        OnListChanged(new(ListChangeAction.Remove, item, currentIndex));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index: currentIndex));
    }

    /// <summary>
    /// 列表清理后
    /// </summary>
    private void OnListCleared()
    {
        OnListChanged(new(ListChangeAction.Clear));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="newValue">新项目</param>
    /// <param name="oldValue">旧项目</param>
    /// <param name="index"></param>
    private void OnListValueChanged(T newValue, T oldValue, int index)
    {
        OnListChanged(new(ListChangeAction.ValueChange, newValue, oldValue, index));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
    }

    /// <summary>
    /// 列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangedEventArgs<T> args)
    {
        ListChanged?.Invoke(args);
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyListChangedEventArgs<T>>? ListChanged;

    #endregion ListChanged

    #region PropertyChanged

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountPropertyChanged()
    {
        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(null, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(null, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged
}
