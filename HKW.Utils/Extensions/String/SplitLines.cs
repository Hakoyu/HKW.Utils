using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 分割行
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>行分割枚举器</returns>
    public static LineSplitEnumerator SplitLines(this string str)
    {
        return new LineSplitEnumerator(str.AsSpan());
    }

    /// <summary>
    /// 分割行
    /// </summary>
    /// <param name="span">字符串</param>
    /// <returns>行分割枚举器</returns>
    public static LineSplitEnumerator SplitLines(this ReadOnlySpan<char> span)
    {
        return new LineSplitEnumerator(span);
    }
}
