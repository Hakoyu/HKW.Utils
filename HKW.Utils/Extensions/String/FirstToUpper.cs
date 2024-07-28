using System.Globalization;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// 扩展集合
/// </summary>
public static partial class HKWExtensions
{
    /// <summary>
    /// 首字母大写
    /// <para><c>red => Red</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="otherToLower">将其余字符变为小写</param>
    /// <returns>首字母大写的字符串</returns>
    public static string FirstToUpper(this string str, bool otherToLower = false)
    {
        if (otherToLower)
        {
            return string.Create(
                str.Length,
                str,
                static (chars, state) =>
                {
                    state.AsSpan().ToLower(chars, null);
                    chars[0] = char.ToUpper(chars[0]);
                }
            );
        }
        else
        {
            return string.Create(
                str.Length,
                str,
                static (chars, state) =>
                {
                    state.CopyTo(chars);
                    chars[0] = char.ToUpper(chars[0]);
                }
            );
        }
    }

    /// <summary>
    /// 首字母大写
    /// <para><c>red => Red</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="culture">文化</param>
    /// <param name="otherToLower">将其余字符变为小写</param>
    /// <returns>首字母大写的字符串</returns>
    public static string FirstToUpper(
        this string str,
        CultureInfo culture,
        bool otherToLower = false
    )
    {
        if (otherToLower)
        {
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.AsSpan().ToLower(chars, culture);
                    chars[0] = char.ToUpper(chars[0], culture);
                }
            );
        }
        else
        {
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.CopyTo(chars);
                    chars[0] = char.ToUpper(chars[0], culture);
                }
            );
        }
    }
}
