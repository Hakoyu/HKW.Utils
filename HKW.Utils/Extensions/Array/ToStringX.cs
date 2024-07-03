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
    /// <param name="array2">数组</param>
    /// <param name="separator">分隔符</param>
    /// <returns>字符串</returns>
    public static string ToStringX<T>(this T[,] array2, string separator = " ")
    {
        var sb = new StringBuilder();
        for (var i = 0; i < array2.GetLength(0); i++)
        {
            for (var j = 0; j < array2.GetLength(1); j++)
            {
                sb.Append(array2[i, j]);
                sb.Append(separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="array2">数组</param>
    /// <param name="getString">获取字符串</param>
    /// <param name="separator">分隔符</param>
    /// <returns>字符串</returns>
    public static string ToStringX<T>(
        this T[,] array2,
        Func<T, string> getString,
        string separator = " "
    )
    {
        var sb = new StringBuilder();
        for (var i = 0; i < array2.GetLength(0); i++)
        {
            for (var j = 0; j < array2.GetLength(1); j++)
            {
                sb.Append(getString(array2[i, j]));
                sb.Append(separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
