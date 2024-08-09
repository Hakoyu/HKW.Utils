namespace HKW.HKWUtils;

/// <summary>
/// 行分割枚举器
/// </summary>
public ref struct LineSplitEnumerator
{
    private ReadOnlySpan<char> _span;

    /// <summary>
    /// 字符串分割设置
    /// </summary>
    private StringSplitOptions _stringSplitOptions;

    /// <summary>
    /// 当前项
    /// </summary>
    public ReadOnlySpan<char> Current { get; private set; }

    /// <inheritdoc/>
    /// <param name="span">字符串</param>
    /// <param name="stringSplitOptions">字符串分割设置</param>
    internal LineSplitEnumerator(ReadOnlySpan<char> span, StringSplitOptions stringSplitOptions)
    {
        _span = span;
        _stringSplitOptions = stringSplitOptions;
    }

    /// <summary>
    /// 将枚举器推进到下一个项目
    /// </summary>
    /// <returns>枚举器是否推进到下一个项目</returns>
    public bool MoveNext()
    {
        if (_span.IsEmpty)
            return false;
        do
        {
            var index = _span.IndexOfAny('\r', '\n');
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
            if (index < _span.Length - 1 && _span[index] == '\r' && _span[index + 1] == '\n')
            {
                // 检测 \r\n
                _span = _span[(index + 2)..];
            }
            else
            {
                _span = _span[(index + 1)..];
            }
        } while (
            Current.IsEmpty && _stringSplitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries)
        );
        return true;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public readonly LineSplitEnumerator GetEnumerator() => this;
}
