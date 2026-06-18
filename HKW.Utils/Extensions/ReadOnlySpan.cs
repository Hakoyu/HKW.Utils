using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    extension<T>(ReadOnlySpan<T> span)
    {
        /// <summary>
        /// 包含索引值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < span.Length;
        }

        /// <summary>
        /// 使用索引获取列表的值或默认值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取成功则返回值, 获取失败则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(int index, T defaultValue)
        {
            if (index >= 0 && index < span.Length)
                return span[index];
            return defaultValue;
        }

        /// <summary>
        /// 尝试使用索引获取区块的值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="value">值</param>
        /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int index, [MaybeNullWhen(false)] out T value)
        {
            if (index >= 0 && index < span.Length)
            {
                value = span[index];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 尝试使用索引获取区块的值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="value">值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValueOrDefault(
            int index,
            [MaybeNullWhen(false)] out T value,
            T defaultValue
        )
        {
            if (index >= 0 && index < span.Length)
            {
                value = span[index];
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }
    }

    /// <summary>
    /// 多分割符分割器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="span">源</param>
    /// <param name="separators">分割符</param>
    /// <returns>枚举器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanSearchValuesSplitEnumerator<T> Split<T>(
        this ReadOnlySpan<T> span,
        SearchValues<T> separators
    )
        where T : IEquatable<T>
    {
        return new SpanSearchValuesSplitEnumerator<T>(span, separators);
    }

    /// <summary>
    /// 多分割符分割器
    /// </summary>
    /// <param name="span">源</param>
    /// <param name="separators">分割符</param>
    /// <param name="options">字符串分割设置</param>
    /// <returns>枚举器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanSearchCharsSplitEnumerator Split(
        this ReadOnlySpan<char> span,
        SearchValues<char> separators,
        StringSplitOptions options
    )
    {
        return new SpanSearchCharsSplitEnumerator(span, separators, options);
    }

    ///// <summary>
    ///// 多分割符分割器
    ///// </summary>
    ///// <param name="span">源</param>
    ///// <param name="separators">分割符集合</param>
    ///// <returns>枚举器</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static SpanSearchStringsSplitEnumerator Split(
    //    this ReadOnlySpan<char> span,
    //    SearchValues<string> separators
    //)
    //{
    //    return new SpanSearchStringsSplitEnumerator(span, separators);
    //}

    /// <summary>
    /// 行分割
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="options">设置</param>
    /// <returns>行分割枚举器</returns>
    public static SpanLineSplitEnumerator SplitLine(
        this ReadOnlySpan<char> span,
        StringSplitOptions options = StringSplitOptions.None
    )
    {
        return new SpanLineSplitEnumerator(span, options);
    }

    private const int StringLengthLimit = 2048;

    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格字符串
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="sourceToLower">将原始字符串转换为小写</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this ReadOnlySpan<char> span,
        char separator,
        bool sourceToLower = true,
        CultureInfo? cultureInfo = null
    )
    {
        bool usePool = span.Length > StringLengthLimit;
        char[] resultArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        char[] sourceArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        Span<char> result = usePool ? resultArray : stackalloc char[span.Length];
        Span<char> source = usePool ? sourceArray : stackalloc char[span.Length];
        cultureInfo ??= CultureInfo.CurrentCulture;
        if (sourceToLower)
            span.ToLower(source, cultureInfo);
        else
            span.CopyTo(source);
        var index = 0;
        foreach (var range in source.Split(separator))
        {
            var splitValue = source[range];
            splitValue.CopyTo(result.Slice(index, splitValue.Length));
            result[index] = cultureInfo.TextInfo.ToUpper(source[range.Start]);
            index += splitValue.Length;
        }
        result = result[..index];
        var resultStr = new string(result);
        if (usePool)
        {
            ArrayPool<char>.Shared.Return(resultArray);
            ArrayPool<char>.Shared.Return(sourceArray);
        }
        return resultStr;
    }

    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格字符串
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="separators">分隔符</param>
    /// <param name="sourceToLower">将原始字符串转换为小写</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this ReadOnlySpan<char> span,
        SearchValues<char> separators,
        bool sourceToLower = true,
        CultureInfo? cultureInfo = null
    )
    {
        bool usePool = span.Length > StringLengthLimit;
        char[] resultArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        char[] sourceArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        Span<char> result = usePool ? resultArray : stackalloc char[span.Length];
        Span<char> source = usePool ? sourceArray : stackalloc char[span.Length];
        cultureInfo ??= CultureInfo.CurrentCulture;
        if (sourceToLower)
            span.ToLower(source, cultureInfo);
        else
            span.CopyTo(source);
        var index = 0;
        foreach (var range in source.Split(separators))
        {
            var splitValue = source[range];
            splitValue.CopyTo(result.Slice(index, splitValue.Length));
            result[index] = cultureInfo.TextInfo.ToUpper(source[range.Start]);
            index += splitValue.Length;
        }
        result = result[..index];
        var resultStr = new string(result);
        if (usePool)
        {
            ArrayPool<char>.Shared.Return(resultArray);
            ArrayPool<char>.Shared.Return(sourceArray);
        }
        return resultStr;
    }

    ///// <summary>
    ///// 将需要分割的字符串转换为帕斯卡风格字符串
    ///// <para><c>red red red => RedRedRed</c></para>
    ///// </summary>
    ///// <param name="span">字符串</param>
    ///// <param name="separators">分隔符</param>
    ///// <param name="sourceToLower">将原始字符串转换为小写</param>
    ///// <param name="cultureInfo">文化信息</param>
    ///// <returns>帕斯卡命名格式的字符串</returns>
    //public static string ToPascal(
    //    this ReadOnlySpan<char> span,
    //    SearchValues<string> separators,
    //    bool sourceToLower = true,
    //    CultureInfo? cultureInfo = null
    //)
    //{
    //    bool usePool = span.Length > StringLengthLimit;
    //    char[] resultArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
    //    char[] sourceArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
    //    Span<char> result = usePool ? resultArray : stackalloc char[span.Length];
    //    Span<char> source = usePool ? sourceArray : stackalloc char[span.Length];
    //    cultureInfo ??= CultureInfo.CurrentCulture;
    //    if (sourceToLower)
    //        span.ToLower(source, cultureInfo);
    //    else
    //        span.CopyTo(source);
    //    var index = 0;
    //    foreach (var range in source.Split(separators))
    //    {
    //        var splitValue = source[range];
    //        splitValue.CopyTo(result.Slice(index, splitValue.Length));
    //        result[index] = cultureInfo.TextInfo.ToUpper(source[range.Start]);
    //        index += splitValue.Length;
    //    }
    //    result = result[..index];
    //    var resultStr = new string(result);
    //    if (usePool)
    //    {
    //        ArrayPool<char>.Shared.Return(resultArray);
    //        ArrayPool<char>.Shared.Return(sourceArray);
    //    }
    //    return resultStr;
    //}
}
