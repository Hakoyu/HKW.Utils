namespace HKW.HKWUtils;

/// <summary>
/// 行分割枚举器
/// </summary>
public ref struct LineSplitEnumerator
{
    private ReadOnlySpan<char> _span;

    /// <inheritdoc/>
    /// <param name="span">字符串</param>
    public LineSplitEnumerator(ReadOnlySpan<char> span)
    {
        _span = span;
        Current = default;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns></returns>
    public readonly LineSplitEnumerator GetEnumerator() => this;

    /// <summary>
    /// 将枚举器推进到 <see cref="ReadOnlySpan{Char}"/> 中的下一个元素
    /// </summary>
    /// <returns>枚举器中是否有另一个项目</returns>
    public bool MoveNext()
    {
        var span = _span;
        if (span.Length == 0) // 已经达到字符串的末端
            return false;

        var index = span.IndexOfAny('\r', '\n');
        // 处理空字符时
        if (index == -1)
        {
            _span = ReadOnlySpan<char>.Empty;
            Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
            return true;
        }
        // 处理换行符
        if (index < span.Length - 1 && span[index] == '\r')
        {
            var next = span[index + 1];
            // 检测 \r\n
            if (next == '\n')
            {
                Current = new LineSplitEntry(span[..index], span.Slice(index, 2));
                _span = span[(index + 2)..];
                return true;
            }
        }

        Current = new LineSplitEntry(span[..index], span.Slice(index, 1));
        _span = span[(index + 1)..];
        return true;
    }

    /// <summary>
    /// 当前
    /// </summary>
    public LineSplitEntry Current { get; private set; }
}
