using System.Collections;
using System.Data;
using System.Diagnostics;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读哈希集合
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
public class ReadOnlySet<T> : ISet<T>, IReadOnlySet<T>
{
    /// <summary>
    /// 原始集合
    /// </summary>
    private readonly ISet<T> _set;

    /// <summary>
    /// 初始化只读集合
    /// </summary>
    /// <param name="set">集合</param>
    /// <exception cref="ArgumentNullException">iSet 为 null</exception>
    public ReadOnlySet(ISet<T> set)
    {
        ArgumentNullException.ThrowIfNull(set);
        _set = set;
    }

    /// <inheritdoc/>
    public int Count => _set.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _set.IsProperSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _set.IsProperSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _set.IsSubsetOf(other);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _set.IsSupersetOf(other);
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        return _set.Overlaps(other);
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        return _set.SetEquals(other);
    }

    /// <inheritdoc/>
    bool ISet<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
