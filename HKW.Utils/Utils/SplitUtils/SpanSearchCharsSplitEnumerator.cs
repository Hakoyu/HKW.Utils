using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils;

/// <summary>
/// 支持枚举由多个 <see langword="char"/> 分隔符根据 <see cref="StringSplitOptions"/> 拆分后的 <see cref="ReadOnlySpan{T}"/> 的每个片段
/// </summary>
public ref struct SpanSearchCharsSplitEnumerator
{
    /// <summary>
    /// 正在被拆分的输入 Span
    /// </summary>
    private readonly ReadOnlySpan<char> _source;

    /// <summary>
    /// 分隔符
    /// </summary>
    private readonly SearchValues<char> _searchValues;

    /// <summary>
    /// 字符串分割设置
    /// </summary>
    private readonly StringSplitOptions _options;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的起始索引（包含）
    /// </summary>
    private int _startCurrent = 0;

    /// <summary>
    /// 当前范围在 <see cref="_source"/> 中的结束索引（不包含）
    /// </summary>
    private int _endCurrent = 0;

    /// <summary>
    /// 在 <see cref="_source"/> 中下一次开始搜索分隔符的索引
    /// </summary>
    private int _startNext = 0;

    /// <summary>
    /// 枚举已结束
    /// </summary>
    private bool _isEnd = false;

    /// <summary>
    /// 获取正在被枚举的源 Span
    /// </summary>
    public readonly ReadOnlySpan<char> Source => _source;

    /// <summary>
    /// 获取枚举中的当前分割的元素范围
    /// </summary>
    public Range Current => new Range(_startCurrent, _endCurrent);

    /// <summary>
    /// 获取枚举中的当前元素
    /// </summary>
    public ReadOnlySpan<char> CurrentValue { get; private set; }

    /// <summary>
    /// 初始化枚举器
    /// </summary>
    /// <param name="source">源Span</param>
    /// <param name="searchValues">搜索值</param>
    /// <param name="options">字符串分割设置</param>
    public SpanSearchCharsSplitEnumerator(
        ReadOnlySpan<char> source,
        SearchValues<char> searchValues,
        StringSplitOptions options
    )
    {
        _source = source;
        _searchValues = searchValues;
        _options = options;
    }

    /// <summary>
    /// 将枚举器推进到下一个元素
    /// </summary>
    /// <returns>
    /// 是否成功
    /// </returns>
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

        const int separatorLength = 1;
        int separatorIndex;

        separatorIndex = _source.Slice(_startNext).IndexOfAny(_searchValues);

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
        const int separatorLength = 1;

        while (_isEnd is false)
        {
            var remaining = _source[_startNext..];
            var separatorIndex = remaining.IndexOfAny(_searchValues);

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

        const int separatorLength = 1;
        int separatorIndex;

        separatorIndex = _source.Slice(_startNext).IndexOfAny(_searchValues);

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
        const int separatorLength = 1;

        while (_isEnd is false)
        {
            var remaining = _source[_startNext..];
            var separatorIndex = remaining.IndexOfAny(_searchValues);

            // 没有更多分隔符则处理最后一段
            if (separatorIndex < 0)
            {
                _startCurrent = _startNext;
                _endCurrent = _source.Length;
                _startNext = _source.Length;
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

    /// <summary>
    /// 获取一个可用于遍历拆分后 Span 的枚举器
    /// </summary>
    public SpanSearchCharsSplitEnumerator GetEnumerator() => this;

    /// <summary>
    /// 重置枚举
    /// </summary>
    public void Reset()
    {
        _startCurrent = 0;
        _endCurrent = 0;
        _startNext = 0;
        _isEnd = false;
    }

    /// <summary>
    /// 转换为列表
    /// </summary>
    /// <typeparam name="TItem">列表项类型</typeparam>
    /// <param name="converter">转换器</param>
    /// <returns>列表</returns>
    public List<TItem> ToList<TItem>(Func<SpanSearchCharsSplitEnumerator, TItem> converter)
    {
        var list = new List<TItem>();
        Reset();
        while (MoveNext())
            list.Add(converter(this));
        Reset();
        return list;
    }
}
