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
public partial class ObservableSelectableSet<T>
    : ObservableSelectableSetWrapper<T, ObservableSet<T>>,
        ISet<T>
    where T : notnull
{
    /// <inheritdoc/>
    public ObservableSelectableSet()
        : base(new()) { }

    /// <inheritdoc/>
    ///  <param name="collection">集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
        : base(new(collection, comparer)) { }

    /// <inheritdoc/>
    /// <param name="capacity">容量</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableSet(int capacity, IEqualityComparer<T>? comparer = null)
        : base(new(capacity, comparer)) { }

    /// <inheritdoc/>
    public int Count => SourceSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceSet.IsReadOnly;

    /// <inheritdoc/>
    public bool Add(T item)
    {
        return SourceSet.Add(item);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        SourceSet.ExceptWith(other);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        SourceSet.IntersectWith(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return SourceSet.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return SourceSet.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return SourceSet.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return SourceSet.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return SourceSet.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return SourceSet.SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        SourceSet.SymmetricExceptWith(other);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        SourceSet.UnionWith(other);
    }

    void ICollection<T>.Add(T item)
    {
        ((ICollection<T>)SourceSet).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceSet.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return SourceSet.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        SourceSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return SourceSet.Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return SourceSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceSet).GetEnumerator();
    }
}
