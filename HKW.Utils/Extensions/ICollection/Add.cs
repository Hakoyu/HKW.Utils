using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 范围添加
    /// </summary>
    /// <typeparam name="TItem">项目类型</typeparam>
    /// <typeparam name="TCollection">集合类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="item">项目</param>
    public static TCollection Add<TItem, TCollection>(this TCollection collection, TItem item)
        where TCollection : ICollection<TItem>
    {
        collection.Add(item);
        return collection;
    }
}
