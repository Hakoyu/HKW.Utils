using System.Globalization;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 转换为大写
    /// <para><c>aaa => AAA</c></para>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="range">范围</param>
    /// <param name="culture">文化</param>
    /// <returns>转换后的字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToUpper(this string str, Range range, CultureInfo? culture = null)
    {
        if (culture is null)
        {
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.CopyTo(chars);
                    var (index, count) = range.GetOffset(state.Length);
                    for (var i = index; i < count; i++)
                        chars[i] = char.ToUpper(chars[i]);
                }
            );
        }
        else
            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.CopyTo(chars);
                    var (index, count) = range.GetOffset(state.Length);
                    for (var i = index; i < count; i++)
                        chars[i] = char.ToUpper(chars[i], culture);
                }
            );
    }
}
