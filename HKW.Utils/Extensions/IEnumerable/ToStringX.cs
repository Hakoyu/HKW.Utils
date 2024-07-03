using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="separator">分隔符</param>
    /// <returns>字符串</returns>
    public static string ToStringX<T>(this IEnumerable<T> source, string separator = " ")
    {
        return string.Join(", ", source);
    }
}
