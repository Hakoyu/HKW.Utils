using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试使用索引获取列表的值
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">索引</param>
    /// <param name="value">项目</param>
    /// <returns>成功获取值为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetValue<T>(
        this IList<T> list,
        int index,
        [MaybeNullWhen(false)] out T value
    )
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        if (index >= 0 && index < list.Count)
        {
            value = list[index];
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}
