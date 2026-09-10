using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class SpanExtensions
{
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="span">内存块</param>
    extension<T>(Span<T> span)
    {
        /// <summary>
        /// 包含索引值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>是否包含</returns>
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
        /// <returns>是否获取成功</returns>
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
        /// <returns>是否获取成功</returns>
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
    /// <param name="separators">分割符集合</param>
    /// <returns>枚举器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanSearchValuesSplitEnumerator<T> Split<T>(
        this Span<T> span,
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
        this Span<char> span,
        SearchValues<char> separators,
        StringSplitOptions options
    )
    {
        return new SpanSearchCharsSplitEnumerator(span, separators, options);
    }

    ///// <summary>
    ///// 多分割符分割器
    ///// </summary>
    ///// <typeparam name="T">类型</typeparam>
    ///// <param name="span">源</param>
    ///// <param name="separators">分割符集合</param>
    ///// <returns>枚举器</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static SpanSearchStringsSplitEnumerator Split<T>(
    //    this Span<char> span,
    //    SearchValues<string> separators
    //)
    //    where T : IEquatable<T>
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
        this Span<char> span,
        StringSplitOptions options = StringSplitOptions.None
    )
    {
        return new SpanLineSplitEnumerator(span, options);
    }

    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格字符串
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="sourceToLower">将原始字符串转换为小写,若确定来源是小写时可关闭以提高性能</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this Span<char> span,
        char separator,
        bool sourceToLower = false,
        CultureInfo? cultureInfo = null
    )
    {
        return ReadOnlySpanExtensions.ToPascal(
            (ReadOnlySpan<char>)span,
            separator,
            sourceToLower,
            cultureInfo
        );
    }

    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格字符串
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="span">字符串</param>
    /// <param name="separators">分隔符</param>
    /// <param name="sourceToLower">将原始字符串转换为小写,若确定来源是小写时可关闭以提高性能</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this Span<char> span,
        SearchValues<char> separators,
        bool sourceToLower = false,
        CultureInfo? cultureInfo = null
    )
    {
        return ReadOnlySpanExtensions.ToPascal(
            (ReadOnlySpan<char>)span,
            separators,
            sourceToLower,
            cultureInfo
        );
    }

    ///// <summary>
    ///// 将需要分割的字符串转换为帕斯卡风格字符串
    ///// <para><c>red red red => RedRedRed</c></para>
    ///// </summary>
    ///// <param name="span">字符串</param>
    ///// <param name="separators">分隔符</param>
    ///// <param name="sourceToLower">将原始字符串转换为小写,若确定来源是小写时可关闭以提高性能</param>
    ///// <param name="cultureInfo">文化信息</param>
    ///// <returns>帕斯卡命名格式的字符串</returns>
    //public static string ToPascal(
    //    this Span<char> span,
    //    SearchValues<string> separators,
    //    bool sourceToLower = false,
    //    CultureInfo? cultureInfo = null
    //)
    //{
    //    return ToPascal((ReadOnlySpan<char>)span, separators, sourceToLower, cultureInfo);
    //}
}
