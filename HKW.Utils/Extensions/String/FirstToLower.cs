using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 首字母小写
    /// <para><c>Red => red</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>首字母小写的字符串</returns>
    public static string FirstToLower(this string str)
    {
        return string.Create(
            str.Length,
            str,
            static (chars, state) =>
            {
                state.CopyTo(chars);
                chars[0] = char.ToLower(chars[0]);
            }
        );
    }

    /// <summary>
    /// 首字母小写
    /// <para><c>Red => red</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="culture">文化</param>
    /// <returns>首字母小写的字符串</returns>
    public static string FirstToLower(
        this string str,
        CultureInfo culture,
        bool otherToLower = false
    )
    {
        return string.Create(
            str.Length,
            str,
            (chars, state) =>
            {
                state.CopyTo(chars);
                chars[0] = char.ToLower(chars[0], culture);
            }
        );
    }
}
