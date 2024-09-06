using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察可选中列表
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
/// <typeparam name="TList">列表</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableSelectableList<T, TList> : ReactiveObjectX, IList<T>, IList
    where TList : IList<T>
{
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    public ObservableSelectableList(TList list)
    {
        List = list;
        PropertyChanged += ObservableSelectableList_PropertyChanged;
    }

    private void ObservableSelectableList_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e
    )
    {
        if (e.PropertyName == nameof(SelectedItem))
        {
            if (_changing)
                return;
            _changing = true;
            SelectedIndex = List.IndexOf(SelectedItem!);
            _changing = false;
        }
        else if (e.PropertyName == nameof(SelectedIndex))
        {
            if (_changing)
                return;
            _changing = true;
            SelectedItem = SelectedIndex < 0 ? default : List[SelectedIndex];
            _changing = false;
        }
    }

    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="seletedIndex">选中项索引</param>
    public ObservableSelectableList(TList list, int seletedIndex)
        : this(list)
    {
        SelectedIndex = seletedIndex;
    }

    /// <inheritdoc/>
    /// <param name="list"></param>
    /// <param name="seletedItem">选中项</param>
    public ObservableSelectableList(TList list, T seletedItem)
        : this(list)
    {
        SelectedItem = seletedItem;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _changing = false;

    /// <summary>
    /// 列表 (使用此属性修改列表不会正确的修改 <see cref="SelectedIndex"/> 和 <see cref="SelectedItem"/>)
    /// </summary>
    public TList List { get; }

    /// <summary>
    /// 选中的索引
    /// </summary>
    [ReactiveProperty]
    public int SelectedIndex { get; set; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public T? SelectedItem { get; set; }

    #region IListT
    /// <inheritdoc/>
    public T this[int index]
    {
        get => ((IList<T>)List)[index];
        set
        {
            ((IList<T>)List)[index] = value;
            if (index == SelectedIndex)
                SelectedItem = value;
        }
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<T>)List).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)List).IsReadOnly;

    /// <inheritdoc/>
    public void Add(T item)
    {
        ((ICollection<T>)List).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<T>)List).Clear();
        SelectedIndex = -1;
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
        if (index <= SelectedIndex)
            SelectedIndex += 1;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var result = List.Remove(item, out var index);
        if (SelectedIndex == index)
        {
            SelectedIndex = -1;
        }
        if (SelectedIndex >= index)
        {
            SelectedIndex -= 1;
        }
        return result;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        ((IList<T>)List).RemoveAt(index);
        if (SelectedIndex == index)
            SelectedIndex = -1;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)List).GetEnumerator();
    }

    #endregion

    #region IList

    object? IList.this[int index]
    {
        get => List[index];
        set
        {
            List[index] = (T)value!;
            if (index == SelectedIndex)
                SelectedItem = (T)value!;
        }
    }
    bool IList.IsFixedSize => ((IList)List).IsFixedSize;

    bool ICollection.IsSynchronized => ((IList)List).IsSynchronized;

    object ICollection.SyncRoot => ((IList)List).SyncRoot;

    int IList.Add(object? value)
    {
        return ((IList)List).Add(value);
    }

    bool IList.Contains(object? value)
    {
        return ((IList)List).Contains(value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((IList)List).CopyTo(array, index);
    }

    int IList.IndexOf(object? value)
    {
        return ((IList)List).IndexOf(value);
    }

    void IList.Insert(int index, object? value)
    {
        ((IList)List).Insert(index, value);
        if (index <= SelectedIndex)
            SelectedIndex += 1;
    }

    void IList.Remove(object? value)
    {
        var result = List.Remove((T)value!, out var index);
        if (SelectedIndex == index)
            SelectedIndex = -1;
    }
    #endregion
}
