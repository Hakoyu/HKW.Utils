using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="item">项目</param>
    /// <returns>项目的索引, 若项目不存在则为 <see langword="-1"/> </returns>
    public static int IndexOf<T>(this IEnumerable<T> source, T item)
    {
        foreach ((var index, var i) in source.EnumerateIndex())
        {
            if (i?.Equals(item) is true)
                return index;
        }
        return -1;
    }

    /// <summary>
    /// 获取索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="match">匹配</param>
    /// <returns>项目的索引, 若项目不存在则为 <see langword="-1"/> </returns>
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> match)
    {
        foreach ((var index, var item) in source.EnumerateIndex())
        {
            if (match(item) is true)
                return index;
        }
        return -1;
    }
}
