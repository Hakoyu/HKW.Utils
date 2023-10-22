using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

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
        if (OnListAdding(list, index))
            return;
        _list.AddRange(list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        var list = new SimpleReadOnlyList<T>(collection);
        if (OnListAdding(list, index))
            return;
        _list.InsertRange(index, list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        var list = new SimpleReadOnlyList<T>(_list.Skip(index).Take(count));
        if (OnListRemoving(list, index))
            return;
        _list.RemoveRange(index, count);
        OnListRemoved(list, index);
    }

    /// <inheritdoc/>
    public void ChangeRange(IEnumerable<T> collection, int index = 0)
    {
        var oldList = new SimpleReadOnlyList<T>(_list.Skip(index));
        var newList = new SimpleReadOnlyList<T>(collection);
        if (OnListValueChanging(newList, oldList, index))
            return;
        for (var i = index; i < oldList.Count; i++)
            _list[i] = newList[i];
        OnListValueChanged(newList, oldList, index);
    }
    #endregion
}
