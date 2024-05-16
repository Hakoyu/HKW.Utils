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
        OnListAdding(list, index);
        _list.AddRange(list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        var list = new SimpleReadOnlyList<T>(collection);
        OnListAdding(list, index);
        _list.InsertRange(index, list);
        OnListAdded(list, index);
    }

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        var list = new SimpleReadOnlyList<T>(_list.Skip(index).Take(count));
        OnListRemoving(list, index);
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
        OnListReplacing(newList, oldList, index);
        _list.Reverse(index, count);
        OnListReplaced(newList, oldList, index);
    }

    #endregion
}
