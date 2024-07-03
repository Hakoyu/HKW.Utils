using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils;

/// <summary>
/// Bool工具
/// </summary>
public static class BoolUtils
{
    /// <summary>
    /// <see langword="true"/>
    /// </summary>
    public const char True = '1';

    /// <summary>
    /// <see langword="false"/>
    /// </summary>
    public const char False = '0';

    /// <summary>
    /// 解析
    /// </summary>
    /// <param name="c">参数</param>
    /// <returns><see langword="bool"/></returns>
    /// <exception cref="ArgumentException">异常参数</exception>
    public static bool Parse(char c)
    {
        if (TryParse(c, out var result) is false)
            throw new ArgumentException($"Bad bool parameter \"{c}\"", nameof(c));
        return result;
    }

    /// <summary>
    /// 尝试解析
    /// </summary>
    /// <param name="c">参数</param>
    /// <param name="result">结果</param>
    /// <returns>解析成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryParse(char c, out bool result)
    {
        result = false;
        if (c == True)
        {
            result = true;
            return true;
        }
        else if (c == False)
        {
            result = false;
            return true;
        }
        return false;
    }
}
