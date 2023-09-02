namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 将需要分割的字符串转换为帕斯卡命名格式
    /// <para><c>red red red => RedRedRed</c></para>
    /// <para>如果不需要分隔则请使用更高性能的 <see cref="FirstLetterCapital(string,bool)"/></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="removeSeparator">删除分隔符</param>
    /// <param name="otherToLower">将其余字符变为小写</param>
    /// <returns>帕斯卡命名格式的字符串</returns>
    public static string ToPascal(
        this string str,
        char separator = ' ',
        bool removeSeparator = true,
        bool otherToLower = false
    )
    {
        var index = 0;
        Span<char> chars = stackalloc char[str.Length];
        Span<char> sourcesSpan = stackalloc char[str.Length];
        if (otherToLower)
            str.AsSpan().ToLower(sourcesSpan, null);
        else
            str.AsSpan().CopyTo(sourcesSpan);
        if (removeSeparator)
        {
            foreach (var word in sourcesSpan.Split(separator))
            {
                word.CopyTo(chars.Slice(index, word.Length));
                chars[index] = char.ToUpper(chars[0]);
                index += word.Length;
            }
            return new string(chars[..index]);
        }
        else
        {
            foreach (var word in sourcesSpan.Split(separator))
            {
                word.CopyTo(chars.Slice(index, word.Length));
                chars[index] = char.ToUpper(chars[0]);
                index += word.Length;
                if (index < sourcesSpan.Length)
                {
                    chars[index] = separator;
                    index++;
                }
            }
            return new string(chars);
        }
    }
}
