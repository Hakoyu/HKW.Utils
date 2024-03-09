namespace HKW.HKWUtils;

/// <summary>
/// Span&lt;char&gt;分割枚举器
/// </summary>
public ref struct CharSpanSplitEnumerator
{
    private ReadOnlySpan<char> _span;

    /// <summary>
    /// 分割项
    /// </summary>
    private readonly char[] _separator;

    /// <summary>
    /// 删除空项
    /// </summary>
    private StringSplitOptions _stringSplitOptions;

    /// <summary>
    /// 当前项
    /// </summary>
    public ReadOnlySpan<char> Current { get; private set; }

    internal CharSpanSplitEnumerator(
        ReadOnlySpan<char> span,
        char[] separator,
        StringSplitOptions stringSplitOptions
    )
    {
        Current = default;
        _span = span;
        _separator = separator;
        _stringSplitOptions = stringSplitOptions;
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
            int index = _span.IndexOfAny(_separator);
            if (index < 0)
            {
                if (_stringSplitOptions.HasFlag(StringSplitOptions.TrimEntries))
                {
                    Current = _span.Trim();
                    _span = ReadOnlySpan<char>.Empty;
                    return !(
                        Current.IsEmpty
                        && _stringSplitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries)
                    );
                }
                else
                {
                    Current = _span;
                    _span = ReadOnlySpan<char>.Empty;
                    return !Current.IsEmpty;
                }
            }
            Current = _stringSplitOptions.HasFlag(StringSplitOptions.TrimEntries)
                ? _span[..index].Trim()
                : _span[..index];
            _span = _span[(index + 1)..];
        } while (
            Current.IsEmpty && _stringSplitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries)
        );
        return true;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public readonly CharSpanSplitEnumerator GetEnumerator() => this;
}
