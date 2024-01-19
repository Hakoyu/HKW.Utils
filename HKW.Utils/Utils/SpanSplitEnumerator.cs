namespace HKW.HKWUtils;

/// <summary>
/// Span分割枚举器
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanSplitEnumerator<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// 只读枚举器源
    /// </summary>
    private ReadOnlySpan<T> _sequence;

    /// <summary>
    /// 分割器
    /// </summary>
    private readonly T _separator;

    /// <summary>
    /// 分割设置
    /// </summary>
    private SpanSplitInfo _spanSplitInfo;

    /// <summary>
    /// 应删除空项
    /// </summary>
    private readonly bool ShouldRemoveEmptyEntries =>
        _spanSplitInfo.HasFlag(SpanSplitInfo.RemoveEmptyEntries);

    /// <summary>
    /// 枚举完成
    /// </summary>
    private readonly bool IsFinished => _spanSplitInfo.HasFlag(SpanSplitInfo.FinishedEnumeration);

    /// <summary>
    /// 获取枚举器当前位置的元素
    /// </summary>
    public ReadOnlySpan<T> Current { get; private set; }

    /// <summary>
    /// 返回当前的枚举器
    /// </summary>
    /// <returns>返回当前的枚举器</returns>
    public readonly SpanSplitEnumerator<T> GetEnumerator() => this;

    internal SpanSplitEnumerator(ReadOnlySpan<T> span, T separator, bool removeEmptyEntries)
    {
        Current = default;
        _sequence = span;
        _separator = separator;
        _spanSplitInfo =
            default(SpanSplitInfo) | (removeEmptyEntries ? SpanSplitInfo.RemoveEmptyEntries : 0);
    }

    /// <summary>
    /// 将枚举器推进到 <see cref="ReadOnlySpan{T}"/> 中的下一个元素
    /// </summary>
    /// <returns>枚举器中是否有另一个项目</returns>
    public bool MoveNext()
    {
        if (IsFinished)
        {
            return false;
        }

        do
        {
            int index = _sequence.IndexOf(_separator);
            if (index < 0)
            {
                Current = _sequence;
                _spanSplitInfo |= SpanSplitInfo.FinishedEnumeration;
                return !(ShouldRemoveEmptyEntries && Current.IsEmpty);
            }

            Current = _sequence[..index];
            _sequence = _sequence[(index + 1)..];
        } while (Current.IsEmpty && ShouldRemoveEmptyEntries);

        return true;
    }

    [Flags]
    private enum SpanSplitInfo : byte
    {
        RemoveEmptyEntries,
        FinishedEnumeration
    }
}
