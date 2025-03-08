using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 添加至末尾, 如果有相同字符串则不添加
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="add">添加的字符串</param>
    /// <returns>处理后的字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AddToEnd(this string str, string add)
    {
        if (str.EndsWith(add))
            return str;
        return str + add;
    }
}
