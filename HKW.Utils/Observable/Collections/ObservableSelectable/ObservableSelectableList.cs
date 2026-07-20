using System.Collections;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public partial class ObservableSelectableList<T>
    : ObservableSelectableListWrapper<T, ObservableList<T>>,
        IList<T>
{
    /// <inheritdoc/>
    public ObservableSelectableList()
        : base(new()) { }

    /// <inheritdoc/>
    public ObservableSelectableList(int capacity)
        : base(new(capacity)) { }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableSelectableList(IEnumerable<T> collection)
        : base(new(collection)) { }

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    /// <inheritdoc/>
    public T this[int index]
    {
        get => SourceList[index];
        set => SourceList[index] = value;
    }

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return SourceList.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        SourceList.Insert(index, item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        SourceList.RemoveAt(index);
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        SourceList.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceList.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return SourceList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        SourceList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return SourceList.Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }
}
