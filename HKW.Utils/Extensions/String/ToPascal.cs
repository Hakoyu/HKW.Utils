using System.Buffers;
using System.Globalization;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡风格名称
    /// <para><c>red red red => RedRedRed</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(this string str, char separator = ' ')
    {
        const int LengthLimit = 2048;
        bool usePool = str.Length > LengthLimit;
        char[] resultArray = usePool ? ArrayPool<char>.Shared.Rent(str.Length) : default!;
        char[] sourcesArray = usePool ? ArrayPool<char>.Shared.Rent(str.Length) : default!;
        Span<char> result = usePool ? resultArray : stackalloc char[str.Length];
        Span<char> sources = usePool ? sourcesArray : stackalloc char[str.Length];
        str.AsSpan().ToLower(sources, CultureInfo.CurrentCulture);
        var index = 0;
        foreach (var word in sources.Split(separator))
        {
            word.CopyTo(result.Slice(index, word.Length));
            result[index] = char.ToUpper(result[0]);
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
