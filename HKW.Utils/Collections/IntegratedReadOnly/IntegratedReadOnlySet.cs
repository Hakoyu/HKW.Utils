using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集成只读集合的集合
/// <para>
/// 示例:
/// <code><![CDATA[
/// IntegratedReadOnlySet<int, HashSet<int>, ReadOnlySet<int>> Set { get; } = new(new (), l => new (l));
/// ReadOnlySet<int> ReadOnlySet => Set.ReadOnlySet;
/// ]]></code>
/// </para>
/// </summary>
/// <typeparam name="T">项类型</typeparam>
/// <typeparam name="TSet">集合类型</typeparam>
/// <typeparam name="TReadOnlySet">只读集合类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class IntegratedReadOnlySet<T, TSet, TReadOnlySet> : ISet<T>, IReadOnlySet<T>
    where TSet : ISet<T>
    where TReadOnlySet : IReadOnlySet<T>
{
    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="readOnlySet">只读集合</param>
    public IntegratedReadOnlySet(TSet set, TReadOnlySet readOnlySet)
    {
        Set = set;
        ReadOnlySet = readOnlySet;
    }

    /// <inheritdoc/>
    /// <param name="set">集合</param>
    /// <param name="getReadOnlySet">获取只读集合</param>
    public IntegratedReadOnlySet(TSet set, Func<TSet, TReadOnlySet> getReadOnlySet)
        : this(set, getReadOnlySet(set)) { }

    /// <summary>
    /// 集合
    /// </summary>
    public ISet<T> Set { get; }

    /// <summary>
    /// 只读集合
    /// </summary>
    public TReadOnlySet ReadOnlySet { get; }

    #region ISet
    /// <inheritdoc/>
    public int Count => Set.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => Set.IsReadOnly;

    /// <inheritdoc/>
    public bool Add(T item)
    {
        return Set.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Set.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return Set.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        Set.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        Set.ExceptWith(other);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return Set.GetEnumerator();
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        Set.IntersectWith(other);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return Set.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return Set.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return Set.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return Set.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return Set.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        return Set.Remove(item);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return Set.SetEquals(other);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        Set.SymmetricExceptWith(other);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        Set.UnionWith(other);
    }

    void ICollection<T>.Add(T item)
    {
        ((ICollection<T>)Set).Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Set).GetEnumerator();
    }
    #endregion
}
