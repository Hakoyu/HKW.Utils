using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 按条件从后往前寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始位置</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public static (int, T?) FindLast<T>(this List<T> list, int startIndex, Predicate<T> match)
    {
        var index = list.FindLastIndex(startIndex, match);
        return (index, list.GetValueOrDefault(index));
    }

    /// <summary>
    /// 按条件从后往前寻找项目和索引
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="startIndex">起始位置</param>
    /// <param name="count">数量</param>
    /// <param name="match">条件</param>
    /// <returns>第一个找到的项目和索引</returns>
    public static (int, T?) FindLast<T>(
        this List<T> list,
        int startIndex,
        int count,
        Predicate<T> match
    )
    {
        var index = list.FindLastIndex(startIndex, count, match);
        return (index, list.GetValueOrDefault(index));
    }
}
