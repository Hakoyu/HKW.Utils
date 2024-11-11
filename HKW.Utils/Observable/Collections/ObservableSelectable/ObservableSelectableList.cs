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
/// 可观测可选中列表
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public partial class ObservableSelectableList<TItem, TList>
    : ReactiveObjectX,
        IList<TItem>,
        IList,
        IListWrapper<TItem, TList>
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    public ObservableSelectableList(TList list)
    {
        BaseList = list;
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
            SelectedIndex = BaseList.IndexOf(SelectedItem!);
            _changing = false;
        }
        else if (e.PropertyName == nameof(SelectedIndex))
        {
            if (_changing)
                return;
            _changing = true;
            SelectedItem = SelectedIndex < 0 ? default : BaseList[SelectedIndex];
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
    public ObservableSelectableList(TList list, TItem seletedItem)
        : this(list)
    {
        SelectedItem = seletedItem;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _changing = false;

    /// <inheritdoc/>
    public TList BaseList { get; }

    /// <summary>
    /// 选中的索引
    /// </summary>
    [ReactiveProperty]
    public int SelectedIndex { get; set; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public TItem? SelectedItem { get; set; }

    #region IListT
    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => ((IList<TItem>)BaseList)[index];
        set
        {
            ((IList<TItem>)BaseList)[index] = value;
            if (index == SelectedIndex)
                SelectedItem = value;
        }
    }

    /// <inheritdoc/>
    public int Count => ((ICollection<TItem>)BaseList).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<TItem>)BaseList).IsReadOnly;

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        ((ICollection<TItem>)BaseList).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<TItem>)BaseList).Clear();
        SelectedIndex = -1;
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
        if (index <= SelectedIndex)
            SelectedIndex += 1;
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var result = BaseList.Remove(item, out var index);
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
        ((IList<TItem>)BaseList).RemoveAt(index);
        if (SelectedIndex == index)
            SelectedIndex = -1;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseList).GetEnumerator();
    }

    #endregion

    #region IList

    object? IList.this[int index]
    {
        get => BaseList[index];
        set
        {
            BaseList[index] = (TItem)value!;
            if (index == SelectedIndex)
                SelectedItem = (TItem)value!;
        }
    }
    bool IList.IsFixedSize => ((IList)BaseList).IsFixedSize;

    bool ICollection.IsSynchronized => ((IList)BaseList).IsSynchronized;

    object ICollection.SyncRoot => ((IList)BaseList).SyncRoot;

    int IList.Add(object? value)
    {
        return ((IList)BaseList).Add(value);
    }

    bool IList.Contains(object? value)
    {
        return ((IList)BaseList).Contains(value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((IList)BaseList).CopyTo(array, index);
    }

    int IList.IndexOf(object? value)
    {
        return ((IList)BaseList).IndexOf(value);
    }

    void IList.Insert(int index, object? value)
    {
        ((IList)BaseList).Insert(index, value);
        if (index <= SelectedIndex)
            SelectedIndex += 1;
    }

    void IList.Remove(object? value)
    {
        var result = BaseList.Remove((TItem)value!, out var index);
        if (SelectedIndex == index)
            SelectedIndex = -1;
    }
    #endregion
}
