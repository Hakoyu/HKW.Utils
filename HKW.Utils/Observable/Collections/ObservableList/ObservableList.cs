using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using System.Collections;
using System.Diagnostics;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableList<T> : ObservableListBase<T>, IList
{
    #region Ctor

    /// <inheritdoc/>
    public ObservableList()
        : base() { }

    /// <inheritdoc/>
    public ObservableList(int capacity)
        : base(capacity) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableList(IEnumerable<T> collection)
        : base(collection) { }

    #endregion Ctor

    #region ILis
    bool IList.IsFixedSize => ((IList)_list).IsFixedSize;
    bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;
    object? IList.this[int index]
    {
        get => _list[index];
        set
        {
            var oldValue = _list[index];
            var oldList = new SimpleSingleItemReadOnlyList<T>(oldValue);
            var newList = new SimpleSingleItemReadOnlyList<T>((T)value!);
            if (oldValue?.Equals(value) is true || OnListValueChanging(newList, oldList, index))
                return;
            _list[index] = (T)value!;
            OnListValueChanged(newList, oldList, index);
        }
    }

    int IList.Add(object? value)
    {
        var item = (T)value!;
        var index = _list.Count - 1;
        Insert(index, item);
        return index;
    }

    void IList.Insert(int index, object? value)
    {
        var item = (T)value!;
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListAdding(list, index))
            return;
        _list.Insert(index, item);
        OnListAdded(list, index);
    }

    void IList.Remove(object? value)
    {
        var item = (T)value!;
        var index = _list.IndexOf(item);
        if (index == -1)
            return;
        var list = new SimpleSingleItemReadOnlyList<T>(item);
        if (OnListRemoving(list, index))
            return;
        _list.RemoveAt(index);
        OnListRemoved(list, index);
    }

    bool IList.Contains(object? value)
    {
        var item = (T)value!;
        return _list.Contains(item);
    }

    int IList.IndexOf(object? value)
    {
        var item = (T)value!;
        return _list.IndexOf(item);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }

    #endregion
}
