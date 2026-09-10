//using System.Buffers;

//namespace HKW.HKWUtils;

///// <summary>
///// 支持枚举由一个或多个分隔符拆分后的 <see cref="ReadOnlySpan{T}"/> 的每个片段。
///// </summary>
//public ref struct SpanSearchStringsSplitEnumerator
//{
//    /// <summary>
//    /// 正在被拆分的输入 Span。
//    /// </summary>
//    private readonly ReadOnlySpan<char> _source;

//    /// <summary>
//    /// 分隔符。
//    /// </summary>
//    private readonly SearchValues<string> _searchValues = default!;

//    /// <summary>
//    /// 当前范围在 <see cref="_source"/> 中的起始索引（包含）。
//    /// </summary>
//    private int _startCurrent = 0;

//    /// <summary>
//    /// 当前范围在 <see cref="_source"/> 中的结束索引（不包含）。
//    /// </summary>
//    private int _endCurrent = 0;

//    /// <summary>
//    /// 在 <see cref="_source"/> 中下一次开始搜索分隔符的索引。
//    /// </summary>
//    private int _startNext = 0;

//    /// <summary>
//    /// 获取一个可用于遍历拆分后 Span 的枚举器。
//    /// </summary>
//    /// <returns>
//    /// 返回一个可用于遍历拆分后 Span 的 <see cref="SpanSearchValuesSplitEnumerator{T}"/>。
//    /// </returns>
//    public SpanSearchStringsSplitEnumerator GetEnumerator() => this;

//    /// <summary>
//    /// 获取正在被枚举的源 Span。
//    /// </summary>
//    /// <returns>
//    /// 返回创建此枚举器时提供的 <see cref="ReadOnlySpan{T}"/>。
//    /// </returns>
//    public readonly ReadOnlySpan<char> Source => _source;

//    /// <summary>
//    /// 获取枚举中的当前元素范围。
//    /// </summary>
//    /// <returns>
//    /// 返回一个 <see cref="Range"/> 实例，表示当前元素在源 Span 中的边界。
//    /// </returns>
//    public Range Current => new Range(_startCurrent, _endCurrent);

//    /// <summary>
//    /// 获取枚举中的当前元素。
//    /// </summary>
//    /// <returns>
//    /// 返回源 Span 中当前元素对应的 <see cref="ReadOnlySpan{T}"/>。
//    /// </returns>
//    public ReadOnlySpan<char> CurrentValue => _source[_startCurrent.._endCurrent];

//    /// <summary>
//    /// 初始化枚举器。
//    /// </summary>
//    internal SpanSearchStringsSplitEnumerator(
//        ReadOnlySpan<char> source,
//        SearchValues<string> searchValues
//    )
//    {
//        _source = source;
//        _searchValues = searchValues;
//    }

//    /// <summary>
//    /// 将枚举器推进到下一个元素。
//    /// </summary>
//    /// <returns>
//    /// 如果成功推进到下一个元素则为 <see langword="true"/>；如果已越过枚举末尾则为 <see langword="false"/>。
//    /// </returns>
//    public bool MoveNext()
//    {
//        // 查找下一个分隔符索引。
//        int separatorIndex,
//            separatorLength;

//        separatorIndex = _source.Slice(_startNext).IndexOfAny(_searchValues);
//        // 无法获取分隔符的长度
//        separatorLength = 1;

//        _startCurrent = _startNext;
//        if (separatorIndex >= 0)
//        {
//            _endCurrent = _startCurrent + separatorIndex;
//            _startNext = _endCurrent + separatorLength;
//            return true;
//        }
//        else
//        {
//            _startNext = _endCurrent = _source.Length;
//            return false;
//        }
//    }
//}
