using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读堆栈
/// </summary>
/// <typeparam name="T">项目类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class ReadOnlyStack<T> : IReadOnlyCollection<T>, ICollection
{
    private readonly Stack<T> _stack;

    /// <inheritdoc/>
    public ReadOnlyStack(Stack<T> stack)
    {
        ArgumentNullException.ThrowIfNull(stack);
        _stack = stack;
    }

    /// <inheritdoc/>
    public int Count => _stack.Count;

    int IReadOnlyCollection<T>.Count => _stack.Count;

    bool ICollection.IsSynchronized => ((ICollection)_stack).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection)_stack).SyncRoot;

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_stack).CopyTo(array, index);
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _stack.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return ((System.Collections.IEnumerable)_stack).GetEnumerator();
    }

    /// <inheritdoc cref="Stack{T}.Contains(T)"/>
    public bool Contains(T item)
    {
        return _stack.Contains(item);
    }

    /// <inheritdoc cref="Stack{T}.CopyTo(T[], int)"/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _stack.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc cref="Stack{T}.Peek"/>
    public T Peek()
    {
        return _stack.Peek();
    }

    /// <inheritdoc cref="Stack{T}.TryPeek(out T)"/>
    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        return _stack.TryPeek(out result);
    }

    /// <inheritdoc cref="Stack{T}.ToArray"/>
    public T[] ToArray()
    {
        return _stack.ToArray();
    }
}
