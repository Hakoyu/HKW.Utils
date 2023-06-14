﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读哈希集合
/// </summary>
/// <typeparam name="T">类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
public class ReadOnlySet<T> : ISet<T>, IReadOnlySet<T>
    where T : notnull
{
    /// <summary>
    /// 异常信息
    /// </summary>
    private const string c_notSupportedExceptionMessage = "Is read only";

    /// <summary>
    /// 原始集合
    /// </summary>
    private readonly ISet<T> r_iSet;

    /// <summary>
    /// 初始化只读集合
    /// </summary>
    /// <param name="iSet">集合</param>
    /// <exception cref="ArgumentNullException">iSet 为 null</exception>
    public ReadOnlySet(ISet<T> iSet)
    {
        ArgumentNullException.ThrowIfNull(iSet);
        r_iSet = iSet;
    }

    /// <inheritdoc/>
    public int Count => r_iSet.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public bool Contains(T item) => r_iSet.Contains(item);

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other) => r_iSet.IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other) => r_iSet.IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other) => r_iSet.IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other) => r_iSet.IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other) => r_iSet.Overlaps(other);

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other) => r_iSet.SetEquals(other);

    /// <inheritdoc/>
    bool ISet<T>.Add(T item)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        r_iSet.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException(c_notSupportedExceptionMessage);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => r_iSet.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
