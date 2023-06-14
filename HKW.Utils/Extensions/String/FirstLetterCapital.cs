using System;
using System.Collections.Generic;
using System.Text;

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
    public static string FirstLetterCapital(this string str, bool otherToLower = false)
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
                    state.AsSpan().CopyTo(chars);
                    chars[0] = char.ToUpper(chars[0]);
                }
            );
        }
    }
}
