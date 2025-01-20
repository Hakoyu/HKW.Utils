using System.Buffers;
using System.Globalization;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格字符串
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="sourceToLower">将原始字符串转换为小写,若确定来源是小写时可关闭以提高性能</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this string str,
        char separator = ' ',
        bool sourceToLower = false,
        CultureInfo? cultureInfo = null
    )
    {
        return ToPascal(str.AsSpan(), separator, sourceToLower, cultureInfo);
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
        this ReadOnlySpan<char> span,
        char separator = ' ',
        bool sourceToLower = true,
        CultureInfo? cultureInfo = null
    )
    {
        const int LengthLimit = 2048;
        bool usePool = span.Length > LengthLimit;
        char[] resultArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        char[] sourcesArray = usePool ? ArrayPool<char>.Shared.Rent(span.Length) : default!;
        Span<char> result = usePool ? resultArray : stackalloc char[span.Length];
        Span<char> sources = usePool ? sourcesArray : stackalloc char[span.Length];
        cultureInfo ??= CultureInfo.CurrentCulture;
        if (sourceToLower)
            span.ToLower(sources, cultureInfo);
        else
            span.CopyTo(sources);
        var index = 0;
        foreach (var word in sources.Split(separator))
        {
            word.CopyTo(result.Slice(index, word.Length));
            result[index] = cultureInfo.TextInfo.ToUpper(word[0]);
            index += word.Length;
        }
        result = result[..index];
        var resultStr = result.ToString();
        if (usePool)
        {
            ArrayPool<char>.Shared.Return(resultArray);
            ArrayPool<char>.Shared.Return(sourcesArray);
        }
        return resultStr;
    }
}
