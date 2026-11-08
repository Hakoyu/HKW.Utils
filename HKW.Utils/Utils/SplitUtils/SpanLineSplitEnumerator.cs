using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils;

/// <summary>
/// 支持枚举由换行符拆分后按 <see cref="StringSplitOptions"/> 分割的 <see cref="ReadOnlySpan{T}"/> 的每个片段
/// </summary>
/// <remarks>
/// <![CDATA[
/// string str = "a\n b\r c\r\n d\n\r e\r\r f\n\n \n";
///
/// split = str.AsSpan().SplitLine(options);
///
/// 选项: StringSplitOptions.None
/// split := [ "a", " b", " c", " d", "", " e", "", " f", "", " ", ""]
///
/// 选项: StringSplitOptions.RemoveEmptyEntries
/// split := [ "a", " b", " c", " d", " e", " f", " "]
///
/// 选项: StringSplitOptions.TrimEntries
/// split := [ "a", "b", "c", "d", "", "e", "", "f", "", "", ""]
///
/// 选项: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
/// split := [ "a", "b", "c", "d", "e", "f"]
/// ]]>
/// </remarks>
public ref struct SpanLineSplitEnumerator
{
    private readonly ReadOnlySpan<char> _source;

    private readonly StringSplitOptions _options;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的起始索引( 包含)
    /// </summary>
    private int _startCurrent = 0;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的结束索引( 不包含)
    /// </summary>
    private int _endCurrent = 0;

    /// <summary>
    /// 在 <see cref="_source"/> 中下次开始查找分隔符的索引
    /// </summary>
    private int _startNext = 0;

    /// <summary>
    /// 枚举已结束
    /// </summary>
    private bool _isEnd = false;

    /// <summary>
    /// 当前范围; 保留源中的原始位置, TrimEntries 不改变此范围.
    /// 仅在 MoveNext 返回 true 后读取; Reset 后范围为 0..0.
    /// </summary>
    public Range Current => new(_startCurrent, _endCurrent);

    /// <summary>
    /// 获取枚举中的当前拆分的片段; TrimEntries 仅修剪此值, 不修改 Current.
    /// 仅在 MoveNext 返回 true 后读取; Reset 后为空.
    /// </summary>
    public ReadOnlySpan<char> CurrentValue { get; private set; }

    /// <summary>
    /// 当前分隔符
    /// </summary>
    public ReadOnlySpan<char> CurrentLineChars => _source[_endCurrent.._startNext];

    /// <summary>
    /// 获取正在被枚举的源 Span
    /// </summary>
    /// <returns>
    /// 返回创建此枚举器时提供的 <see cref="ReadOnlySpan{T}"/>
    /// </returns>
    public readonly ReadOnlySpan<char> Source => _source;

    /// <inheritdoc/>
    /// <param name="span">源 Span</param>
    /// <param name="options">字符串分割设置</param>
    internal SpanLineSplitEnumerator(ReadOnlySpan<char> span, StringSplitOptions options)
    {
        if (
            (options & ~(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            != 0
        )
            throw new ArgumentException("Unsupported split options.", nameof(options));
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

    /// <summary>
    /// 重置枚举, 同时清空当前范围, 值及换行分隔符.
    /// </summary>
    public void Reset()
    {
        _startCurrent = 0;
        _endCurrent = 0;
        _startNext = 0;
        _isEnd = false;
        CurrentValue = default;
    }

    /// <summary>
    /// 转换为列表; 始终从头枚举, 成功或转换器抛出异常后均重置枚举器.
    /// </summary>
    /// <typeparam name="TItem">列表项类型</typeparam>
    /// <param name="converter">转换器</param>
    /// <returns>列表</returns>
    public List<TItem> ToList<TItem>(Func<SpanLineSplitEnumerator, TItem> converter)
    {
        Reset();
        try
        {
            ArgumentNullException.ThrowIfNull(converter);
            var list = new List<TItem>();
            while (MoveNext())
                list.Add(converter(this));
            return list;
        }
        finally
        {
            Reset();
        }
    }
}
