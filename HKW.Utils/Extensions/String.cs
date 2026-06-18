using System.Globalization;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <param name="str">字符串</param>
    extension(string str)
    {
        /// <summary>
        /// 包含索引值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < str.Length;
        }

        /// <summary>
        /// 使用索引获取列表的值或默认值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取成功则返回值, 获取失败则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char GetValueOrDefault(int index, char defaultValue = default)
        {
            if (index >= 0 && index < str.Length)
                return str[index];
            return defaultValue;
        }

        /// <summary>
        /// 添加至开头, 如果有相同字符串则不添加
        /// </summary>
        /// <param name="add">添加的字符串</param>
        /// <returns>处理后的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string EnsureStartsWith(string add)
        {
            if (str.StartsWith(add))
                return str;
            return add + str;
        }

        /// <summary>
        /// 添加至末尾, 如果有相同字符串则不添加
        /// </summary>
        /// <param name="add">添加的字符串</param>
        /// <returns>处理后的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string EnsureEndsWith(string add)
        {
            if (str.EndsWith(add))
                return str;
            return str + add;
        }

        /// <summary>
        /// 首字母小写
        /// <para><c>"Red" => "red"</c></para>
        /// </summary>
        /// <param name="culture">文化</param>
        /// <returns>首字母小写的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FirstToLower(CultureInfo? culture = null)
        {
            if (culture is null)
            {
                return string.Create(
                    str.Length,
                    str,
                    static (chars, state) =>
                    {
                        state.CopyTo(chars);
                        chars[0] = CultureInfo.CurrentCulture.TextInfo.ToLower(chars[0]);
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
                        chars[0] = char.ToLower(chars[0], culture);
                    }
                );
            }
        }

        /// <summary>
        /// 首字母大写
        /// <para><c>"red" => "Red"</c></para>
        /// </summary>
        /// <param name="otherToLower">将其余字符变为小写</param>
        /// <returns>首字母大写的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FirstToUpper(bool otherToLower = false)
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
        /// <para><c>"red" => "Red"</c></para>
        /// </summary>
        /// <param name="culture">文化</param>
        /// <param name="otherToLower">将其余字符变为小写</param>
        /// <returns>首字母大写的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FirstToUpper(CultureInfo culture, bool otherToLower = false)
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

        /// <summary>
        /// 转换为小写
        /// <para><c>"AAA" => "aaa"</c></para>
        /// </summary>
        /// <param name="range">范围</param>
        /// <param name="culture">文化</param>
        /// <returns>转换后的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToLower(Range range, CultureInfo? culture = null)
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
                            chars[i] = char.ToLower(chars[i]);
                    }
                );
            }

            return string.Create(
                str.Length,
                str,
                (chars, state) =>
                {
                    state.CopyTo(chars);
                    var (index, count) = range.GetOffset(state.Length);
                    for (var i = index; i < count; i++)
                        chars[i] = char.ToLower(chars[i], culture);
                }
            );
        }

        /// <summary>
        /// 转换为大写
        /// <para><c>"aaa" => "AAA"</c></para>
        /// </summary>
        /// <param name="range">范围</param>
        /// <param name="culture">文化</param>
        /// <returns>转换后的字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToUpper(Range range, CultureInfo? culture = null)
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
}
