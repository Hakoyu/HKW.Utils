using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 表示相同指定类型的实例可变大小的后进先出 (LIFO) 集合接口
/// </summary>
internal interface IStack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
{
    /// <inheritdoc cref="Stack{T}.Push(T)"/>
    public void Push(T item);

    /// <inheritdoc cref="Stack{T}.Peek"/>
    public T Peek();

    /// <inheritdoc cref="Stack{T}.TryPeek(out T)"/>
    public bool TryPeek([MaybeNullWhen(false)] out T result);

    /// <inheritdoc cref="Stack{T}.Pop"/>
    public T Pop();

    /// <inheritdoc cref="Stack{T}.TryPop(out T)"/>
    public bool TryPop([MaybeNullWhen(false)] out T result);

    /// <inheritdoc cref="Stack{T}.Clear"/>
    public void Clear();

    /// <inheritdoc cref="Stack{T}.Contains(T)"/>
    public bool Contains(T item);
}
