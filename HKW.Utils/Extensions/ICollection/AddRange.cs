using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 范围添加
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="items">项目</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        if (collection is List<T> list)
        {
            list.AddRange(items);
        }
        else
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }

    ///// <summary>
    ///// 删除全部符合条件的项目
    ///// </summary>
    ///// <typeparam name="T">项类型</typeparam>
    ///// <param name="collection">集合</param>
    ///// <param name="match">条件</param>
    //public static void RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
    //{
    //    if (collection is IList<T> list)
    //    {
    //        list.RemoveAll(match);
    //    }
    //    else
    //    {
    //        new HashSet<int>().RemoveWhere();
    //    }
    //}
}
