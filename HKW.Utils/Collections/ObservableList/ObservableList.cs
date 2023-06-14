using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
public class ObservableList<T> : IObservableList<T>
    where T : notnull
{
    /// <summary>
    /// 原始列表
    /// </summary>
    private readonly List<T> r_list;

    #region crot

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

    #endregion

    #region IListT
    /// <inheritdoc/>
    public int Count => r_list.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)r_list).IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public T this[int index]
    {
        get => r_list[index];
        set
        {
            var oldValue = r_list[index];
            if (oldValue.Equals(value) || OnListValueChanging(oldValue, value, index))
                return;
            r_list[index] = value;
            OnListValueChanged(oldValue, value, index);
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
    #endregion
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
    #endregion
    #region IList
    bool IList.IsFixedSize => ((IList)r_list).IsFixedSize;

    bool ICollection.IsSynchronized => ((IList)r_list).IsSynchronized;

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
    #endregion
    #region ListChanging
    /// <summary>
    /// 列表添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListAdding(T item, int? index = null)
    {
        if (ListChanging is not null)
            return OnListChanging(new(NotifyListChangeAction.Add, item, index ?? Count));
        return false;
    }

    /// <summary>
    /// 列表删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <param name="index">索引</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListRemoving(T item, int? index = null)
    {
        if (ListChanging is not null)
            return OnListChanging(new(NotifyListChangeAction.Remove, item, index ?? Count - 1));
        return false;
    }

    /// <summary>
    /// 列表清理时
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListClearing()
    {
        if (ListChanging is not null)
            return OnListChanging(new(NotifyListChangeAction.Clear));
        return false;
    }

    /// <summary>
    /// 列表值改变时
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    /// <param name="index"></param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnListValueChanging(T oldValue, T newValue, int index)
    {
        if (ListChanging is not null)
            return OnListChanging(
                new(NotifyListChangeAction.ValueChange, oldValue, newValue, index)
            );
        return false;
    }

    /// <summary>
    /// 列表改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual bool OnListChanging(NotifyListChangingEventArgs<T> args)
    {
        ListChanging?.Invoke(this, args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event NotifyListChangingEventHandler<T>? ListChanging;
    #endregion
    #region ListChanged

    /// <summary>
    /// 列表已添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <param name="index">索引</param>
    private void OnListAdded(T item, int? index = null)
    {
        var currentIndex = index ?? Count - 1;
        if (ListChanged is not null)
            OnListChanged(new(NotifyListChangeAction.Add, item, currentIndex));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index: currentIndex));
    }

    /// <summary>
    /// 列表已删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <param name="index">索引</param>
    private void OnListRemoved(T item, int? index = null)
    {
        var currentIndex = index ?? Count;
        if (ListChanged is not null)
            OnListChanged(new(NotifyListChangeAction.Remove, item, currentIndex));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Remove, item, index: currentIndex)
            );
    }

    /// <summary>
    /// 列表已清理时
    /// </summary>
    private void OnListCleared()
    {
        if (ListChanged is not null)
            OnListChanged(new(NotifyListChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 列表值已改变
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    /// <param name="index"></param>
    private void OnListValueChanged(T oldValue, T newValue, int index)
    {
        if (ListChanged is not null)
            OnListChanged(new(NotifyListChangeAction.ValueChange, oldValue, newValue, index));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newValue, oldValue, index)
            );
    }

    /// <summary>
    /// 列表已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangedEventArgs<T> args)
    {
        ListChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyListChangedEventHandler<T>? ListChanged;
    #endregion
    #region PropertyChanged

    /// <summary>
    /// 数量已改变时
    /// </summary>
    private void OnCountPropertyChanged()
    {
        if (PropertyChanged is not null)
            OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// 属性已改变时
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
    #region CollectionChanged

    /// <summary>
    /// 列表已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    #endregion
}
