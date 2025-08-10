using System.Buffers;
using System.Globalization;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
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
        return ToPascal((ReadOnlySpan<char>)span, separator, sourceToLower, cultureInfo);
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
        return ToPascal((ReadOnlySpan<char>)span, separators, sourceToLower, cultureInfo);
    }
}
