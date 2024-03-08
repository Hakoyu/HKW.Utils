using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 高级可观测列表
/// <para>
/// 提供了 <see cref="AddRange"/> 等额外方法 但未实现 <see cref="IList"/>
/// </para>
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableListX<T> : ObservableListBase<T>, IObservableListX<T>
{
    #region Ctor

    /// <inheritdoc/>
    public ObservableListX()
        : base() { }

    /// <inheritdoc/>
    public ObservableListX(int capacity)
        : base(capacity) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableListX(IEnumerable<T> collection)
        : base(collection) { }

    #endregion

    #region ChangeRange

    /// <inheritdoc/>
    public void AddRange(IEnumerable<T> collection)
    {
        var index = _list.Count;
        var list = new SimpleReadOnlyList<T>(collection);
        if (OnListAdding(list, index) is false)
            return;
        _list.AddRange(list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        var list = new SimpleReadOnlyList<T>(collection);
        if (OnListAdding(list, index) is false)
            return;
        _list.InsertRange(index, list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        var list = new SimpleReadOnlyList<T>(_list.Skip(index).Take(count));
        if (OnListRemoving(list, index) is false)
            return;
        _list.RemoveRange(index, count);
        OnListRemoved(list, index);
    }

    /// <inheritdoc/>
    public void Reverse()
    {
        Reverse(0, _list.Count);
    }

    /// <inheritdoc/>
    public void Reverse(int index, int count)
    {
        var oldList = new SimpleReadOnlyList<T>(_list.ToList());
        var tempList = _list.ToList();
        tempList.Reverse(index, count);
        var newList = new SimpleReadOnlyList<T>(tempList);
        if (OnListValueChanging(newList, oldList, index) is false)
            return;
        _list.Reverse(index, count);
        OnListValueChanged(newList, oldList, index);
    }

    /// <inheritdoc/>
    public void ChangeRange(IEnumerable<T> collection, int index = 0)
    {
        var oldList = new SimpleReadOnlyList<T>(index >= 0 ? _list.Skip(index) : _list.ToList());
        var newList = new SimpleReadOnlyList<T>(collection);
        if (OnListValueChanging(newList, oldList, index) is false)
            return;
        for (var i = index; i < oldList.Count; i++)
            _list[i] = newList[i];
        OnListValueChanged(newList, oldList, index);
    }

    #endregion
    void IListRange<T>.RemoveAll(Predicate<T> match)
    {
        throw new NotImplementedException();
    }
}
