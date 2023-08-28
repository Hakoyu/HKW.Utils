using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测堆栈
/// </summary>
[DebuggerDisplay("Count = {Count}")]
internal class ObservableStack<T> : IStack<T>
{
    private readonly Stack<T> r_stack = new();

    #region Ctor
    /// <inheritdoc/>
    public ObservableStack()
    {
        r_stack = new();
    }

    /// <inheritdoc/>
    public ObservableStack(int capacity)
    {
        r_stack = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableStack(IEnumerable<T> collection)
    {
        r_stack = new(collection);
    }
    #endregion
    #region IStack
    public int Count => r_stack.Count;

    public bool IsSynchronized => ((ICollection)r_stack).IsSynchronized;

    public object SyncRoot => ((ICollection)r_stack).SyncRoot;

    public T Peek()
    {
        var result = r_stack.Peek();
        return result;
    }

    public T Pop()
    {
        var result = r_stack.Pop();
        return result;
    }

    public void Push(T item)
    {
        r_stack.Push(item);
    }

    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        var temp = r_stack.TryPeek(out result);
        return temp;
    }

    public bool TryPop([MaybeNullWhen(false)] out T result)
    {
        var temp = r_stack.TryPop(out result);
        return temp;
    }

    public void Clear()
    {
        r_stack.Clear();
    }

    public bool Contains(T item)
    {
        return r_stack.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        r_stack.CopyTo(array, index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)r_stack).CopyTo(array, index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return r_stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return r_stack.GetEnumerator();
    }
    #endregion
}
