using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

/// <summary>
/// 字符串工具
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// 行分割符
    /// </summary>
    public static string[] LineSeparator { get; } = ["\r\n", "\r", "\n"];

    /// <summary>
    /// 添加后缀
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="suffix">后缀</param>
    /// <param name="separator">分割符</param>
    /// <returns>带有后缀的字符串</returns>
    public static string AddSuffix(this string str, string suffix, string separator = "_")
    {
        return $"{str}{separator}{suffix}";
    }
}
