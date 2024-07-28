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
    /// 转换为小写
    /// <para><c>AAA => aaa</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="range">范围</param>
    /// <param name="culture">文化</param>
    /// <returns>转换后的字符串</returns>
    public static string ToLower(this string str, Range range, CultureInfo? culture = null)
    {
        if (culture is null)
        {
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.AsSpan().CopyTo(chars);
                    var (index, count) = range.GetOffset(state.Length);
                    for (var i = index; i < count; i++)
                        chars[i] = char.ToLower(chars[i]);
                }
            );
        }
        else
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.AsSpan().CopyTo(chars);
                    var (index, count) = range.GetOffset(state.Length);
                    for (var i = index; i < count; i++)
                        chars[i] = char.ToLower(chars[i], culture);
                }
            );
    }
}
