using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观测堆栈
/// </summary>
[DebuggerDisplay("Count = {Count}")]
internal class ObservableStack<T> : IStack<T>
{
    private readonly Stack<T> _stack = new();

    #region Ctor

    /// <inheritdoc/>
    public ObservableStack()
    {
        _stack = new();
    }

    /// <inheritdoc/>
    public ObservableStack(int capacity)
    {
        _stack = new(capacity);
    }

    /// <inheritdoc/>
    /// <param name="collection">集合</param>
    public ObservableStack(IEnumerable<T> collection)
    {
        _stack = new(collection);
    }

    #endregion Ctor

    #region IStack

    public int Count => _stack.Count;

    public bool IsSynchronized => ((ICollection)_stack).IsSynchronized;

    public object SyncRoot => ((ICollection)_stack).SyncRoot;

    public T Peek()
    {
        var result = _stack.Peek();
        return result;
    }

    public T Pop()
    {
        var result = _stack.Pop();
        return result;
    }

    public void Push(T item)
    {
        _stack.Push(item);
    }

    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        var temp = _stack.TryPeek(out result);
        return temp;
    }

    public bool TryPop([MaybeNullWhen(false)] out T result)
    {
        var temp = _stack.TryPop(out result);
        return temp;
    }

    public void Clear()
    {
        _stack.Clear();
    }

    public bool Contains(T item)
    {
        return _stack.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        _stack.CopyTo(array, index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_stack).CopyTo(array, index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _stack.GetEnumerator();
    }

    #endregion IStack
}
