using System.Buffers;

namespace HKW.HKWUtils;

/// <summary>
/// Span分割枚举器
/// </summary>
/// <typeparam name="T">类型</typeparam>
public ref struct SpanSplitEnumerator<T>
    where T : IEquatable<T>
{
    private ReadOnlySpan<T> _span;

    /// <summary>
    /// 分割项
    /// </summary>
    private readonly T _separator;

    /// <summary>
    /// 删除空项
    /// </summary>
    private readonly bool _removeEmptyEntries;

    /// <summary>
    /// 当前项
    /// </summary>
    public ReadOnlySpan<T> Current { get; private set; }

    internal SpanSplitEnumerator(ReadOnlySpan<T> span, T separator, bool removeEmptyEntries)
    {
        Current = default;
        _span = span;
        _separator = separator;
        _removeEmptyEntries = removeEmptyEntries;
    }

    /// <summary>
    /// 将枚举器将枚举器推进到下一个项目
    /// </summary>
    /// <returns>枚举器是否推进到下一个项目</returns>
    public bool MoveNext()
    {
        if (_span.IsEmpty)
            return false;
        do
        {
            int index = _span.IndexOf(_separator);
            if (index < 0)
            {
                Current = _span;
                _span = ReadOnlySpan<T>.Empty;
                return !Current.IsEmpty;
            }
            Current = _span[..index];
            _span = _span[(index + 1)..];
        } while (Current.IsEmpty && _removeEmptyEntries);
        return true;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public readonly SpanSplitEnumerator<T> GetEnumerator() => this;
}

/// <summary>
/// Span多分割项分割枚举器
/// </summary>
/// <typeparam name="T">类型</typeparam>
public ref struct SpanMultiSeparatorSplitEnumerator<T>
    where T : IEquatable<T>
{
    private ReadOnlySpan<T> _span;

    /// <summary>
    /// 分割项
    /// </summary>
    private readonly SearchValues<T> _separators;

    /// <summary>
    /// 删除空项
    /// </summary>
    private readonly bool _removeEmptyEntries;

    /// <summary>
    /// 当前项
    /// </summary>
    public ReadOnlySpan<T> Current { get; private set; }

    internal SpanMultiSeparatorSplitEnumerator(
        ReadOnlySpan<T> span,
        SearchValues<T> separators,
        bool removeEmptyEntries
    )
    {
        Current = default;
        _span = span;
        _separators = separators;
        _removeEmptyEntries = removeEmptyEntries;
    }

    /// <summary>
    /// 将枚举器将枚举器推进到下一个项目
    /// </summary>
    /// <returns>枚举器是否推进到下一个项目</returns>
    public bool MoveNext()
    {
        if (_span.IsEmpty)
            return false;
        do
        {
            var index = _span.IndexOfAny(_separators);
            if (index < 0)
            {
                Current = _span;
                _span = ReadOnlySpan<T>.Empty;
                return !Current.IsEmpty;
            }
            Current = _span[..index];
            _span = _span[(index + 1)..];
        } while (Current.IsEmpty && _removeEmptyEntries);
        return true;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public readonly SpanMultiSeparatorSplitEnumerator<T> GetEnumerator() => this;
}
