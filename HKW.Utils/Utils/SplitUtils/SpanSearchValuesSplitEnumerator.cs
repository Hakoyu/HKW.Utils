using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils;

/// <summary>
/// 支持枚举由一个或多个分隔符拆分后的 <see cref="ReadOnlySpan{T}"/> 的每个片段
/// </summary>
/// <typeparam name="T"><see cref="SpanSearchValuesSplitEnumerator{T}"/> 中元素的类型</typeparam>
public ref struct SpanSearchValuesSplitEnumerator<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// 正在被拆分的输入 Span
    /// </summary>
    private readonly ReadOnlySpan<T> _source;

    /// <summary>
    /// 分隔符
    /// </summary>
    private readonly SearchValues<T> _searchValues = default!;

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
    /// 获取一个可用于遍历拆分后 Span 的枚举器
    /// </summary>
    public SpanSearchValuesSplitEnumerator<T> GetEnumerator() => this;

    /// <summary>
    /// 获取正在被枚举的源 Span
    /// </summary>
    public readonly ReadOnlySpan<T> Source => _source;

    /// <summary>
    /// 获取枚举中的当前分割的元素范围
    /// </summary>
    public Range Current => new Range(_startCurrent, _endCurrent);

    /// <summary>
    /// 获取枚举中的当前元素
    /// </summary>
    public ReadOnlySpan<T> CurrentValue => _source[_startCurrent.._endCurrent];

    /// <summary>
    /// 获取枚举中的当前分隔符当枚举到最后一个元素时, 分隔符为 <see langword="default"/>(T)
    /// </summary>
    public T CurrentSeparator => _endCurrent < _source.Length ? _source[_endCurrent] : default!;

    /// <summary>
    /// 初始化枚举器
    /// </summary>
    internal SpanSearchValuesSplitEnumerator(ReadOnlySpan<T> source, SearchValues<T> searchValues)
    {
        _source = source;
        _searchValues = searchValues;
    }

    /// <summary>
    /// 将枚举器推进到下一个元素
    /// </summary>
    /// <returns>
    /// 是否成功
    /// </returns>
    public bool MoveNext()
    {
        if (_isEnd)
            return false;
        // 查找下一个分隔符索引
        int separatorIndex,
            separatorLength = 1;

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
        return true;
    }

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
    public List<TItem> ToList<TItem>(Func<SpanSearchValuesSplitEnumerator<T>, TItem> converter)
    {
        var list = new List<TItem>();
        Reset();
        while (MoveNext())
            list.Add(converter(this));
        Reset();
        return list;
    }
}
