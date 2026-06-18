using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils;

/// <summary>
/// 支持枚举由换行符拆分后按 <see cref="StringSplitOptions"/> 分割的 <see cref="ReadOnlySpan{T}"/> 的每个片段。
/// </summary>
/// <remarks>
/// <![CDATA[
/// string str = "a\n b\r c\r\n d\n\r e\r\r f\n\n \n";
///
/// split = string.AsSpan().SplitLine(StringSplitOptions);
///
/// Options: StringSplitOptions.None
/// split := [ "a", " b", " c", " d", "", " e", "", " f", "", " "]
///
/// Options: StringSplitOptions.RemoveEmptyEntries
/// split := [ "a", " b", " c", " d", " e", " f", " "]
///
/// Options: StringSplitOptions.TrimEntries
/// split := [ "a", "b", "c", "d", "", "e", "", "f", ""]
///
/// Options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
/// split := [ "a", "b", "c", "d", "e", "f"]
/// ]]>
/// </remarks>
public ref struct SpanLineSplitEnumerator
{
    private readonly ReadOnlySpan<char> _source;

    private readonly StringSplitOptions _options;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的起始索引（包含）。
    /// </summary>
    private int _startCurrent = 0;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的结束索引（不包含）。
    /// </summary>
    private int _endCurrent = 0;

    /// <summary>
    /// 在 <see cref="_source"/> 中下次开始查找分隔符的索引。
    /// </summary>
    private int _startNext = 0;

    /// <summary>
    /// 枚举已结束
    /// </summary>
    private bool _isEnd = false;

    /// <summary>
    /// 当前范围
    /// </summary>
    public Range Current => new(_startCurrent, _endCurrent);

    /// <summary>
    /// 获取枚举中的当前拆分的片段。
    /// </summary>
    public ReadOnlySpan<char> CurrentValue { get; private set; }

    /// <summary>
    /// 当前分隔符
    /// </summary>
    public ReadOnlySpan<char> CurrentLineChars => _source[_endCurrent.._startNext];

    /// <summary>
    /// 获取正在被枚举的源 Span。
    /// </summary>
    /// <returns>
    /// 返回创建此枚举器时提供的 <see cref="ReadOnlySpan{T}"/>。
    /// </returns>
    public readonly ReadOnlySpan<char> Source => _source;

    /// <inheritdoc/>
    /// <param name="span">源 Span</param>
    /// <param name="options">字符串分割设置</param>
    internal SpanLineSplitEnumerator(ReadOnlySpan<char> span, StringSplitOptions options)
    {
        _source = span;
        _options = options;
    }

    /// <summary>
    /// 将枚举器推进到下一个项目
    /// </summary>
    /// <returns>枚举器是否推进到下一个项目</returns>
    public bool MoveNext()
    {
        return _options switch
        {
            StringSplitOptions.None => MoveNext_None(),
            StringSplitOptions.RemoveEmptyEntries => MoveNext_RemoveEmptyEntries(),
            StringSplitOptions.TrimEntries => MoveNext_TrimEntries(),
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries =>
                MoveNext_TrimAndRemoveEmptyEntries(),
            _ => false,
        };
    }

    private bool MoveNext_None()
    {
        if (_isEnd)
            return false;

        var separatorIndex = SpanLineSplitEnumerator.IndexOfLine(
            _source.Slice(_startNext),
            out var separatorLength
        );

        _startCurrent = _startNext;
        if (separatorIndex >= 0)
        {
            _endCurrent = _startCurrent + separatorIndex;
            _startNext = _endCurrent + separatorLength;
        }
        else
        {
            _startNext = _endCurrent = _source.Length;
            _isEnd = true;
        }
        CurrentValue = _source[_startCurrent.._endCurrent];
        return true;
    }

    private bool MoveNext_RemoveEmptyEntries()
    {
        while (_isEnd is false)
        {
            var separatorIndex = SpanLineSplitEnumerator.IndexOfLine(
                _source.Slice(_startNext),
                out var separatorLength
            );

            // 没有更多分隔符则处理最后一段
            if (separatorIndex < 0)
            {
                _startCurrent = _startNext;
                _startNext = _endCurrent = _source.Length;
                _isEnd = true;

                var tail = _source[_startCurrent.._endCurrent];
                if (tail.Length == 0)
                    return false;

                CurrentValue = tail;
                return true;
            }

            // 命中分隔符则先截取当前段
            _startCurrent = _startNext;
            _endCurrent = _startCurrent + separatorIndex;
            _startNext = _endCurrent + separatorLength;

            var value = _source[_startCurrent.._endCurrent];
            if (value.Length != 0)
            {
                CurrentValue = value;
                return true;
            }

            // 当前段为空或空白则继续找下一段
        }
        return false;
    }

    private bool MoveNext_TrimEntries()
    {
        if (_isEnd)
            return false;

        var separatorIndex = SpanLineSplitEnumerator.IndexOfLine(
            _source.Slice(_startNext),
            out var separatorLength
        );

        _startCurrent = _startNext;
        if (separatorIndex >= 0)
        {
            _endCurrent = _startCurrent + separatorIndex;
            _startNext = _endCurrent + separatorLength;
        }
        else
        {
            _startNext = _endCurrent = _source.Length;
            _isEnd = true;
        }
        CurrentValue = _source[_startCurrent.._endCurrent].Trim();
        return true;
    }

    private bool MoveNext_TrimAndRemoveEmptyEntries()
    {
        while (_isEnd is false)
        {
            var separatorIndex = SpanLineSplitEnumerator.IndexOfLine(
                _source.Slice(_startNext),
                out var separatorLength
            );

            // 没有更多分隔符则处理最后一段
            if (separatorIndex < 0)
            {
                _startCurrent = _startNext;
                _startNext = _endCurrent = _source.Length;
                _isEnd = true;

                var tail = _source[_startCurrent.._endCurrent].Trim();
                if (tail.Length == 0)
                    return false;

                CurrentValue = tail;
                return true;
            }

            // 命中分隔符则先截取当前段
            _startCurrent = _startNext;
            _endCurrent = _startCurrent + separatorIndex;
            _startNext = _endCurrent + separatorLength;

            var value = _source[_startCurrent.._endCurrent].Trim();
            if (value.Length != 0)
            {
                CurrentValue = value;
                return true;
            }

            // 当前段为空或空白则继续找下一段
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfLine(ReadOnlySpan<char> span, out int separatorLength)
    {
        var index = span.IndexOfAny('\r', '\n');
        separatorLength = 1;

        if (index < 0)
            return index;

        if (span[index] == '\r' && index + 1 < span.Length && span[index + 1] == '\n')
        {
            separatorLength = 2;
        }
        return index;
    }

    /// <summary>
    /// 枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public readonly SpanLineSplitEnumerator GetEnumerator() => this;

    /// <inheritdoc/>
    public void Reset()
    {
        _startCurrent = 0;
        _endCurrent = 0;
        _startNext = 0;
        _isEnd = false;
        CurrentValue = default;
    }

    /// <summary>
    /// 转换为列表
    /// </summary>
    /// <typeparam name="TItem">列表项类型</typeparam>
    /// <param name="converter">转换器</param>
    /// <returns>列表</returns>
    public List<TItem> ToList<TItem>(Func<SpanLineSplitEnumerator, TItem> converter)
    {
        var list = new List<TItem>();
        Reset();
        while (MoveNext())
            list.Add(converter(this));
        Reset();
        return list;
    }
}
