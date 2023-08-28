using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试使用索引获取列表的值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="list">集合</param>
    /// <param name="index">索引值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>获取成功则返回值, 获取失败则返回默认值</returns>
    public static T? GetValueOrDefault<T>(this IList<T> list, int index, T? defaultValue = default)
    {
        if (index >= 0 && index < list.Count)
            return list[index];
        return defaultValue;
    }
}
