using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察扩展
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// 转换至可观察列表
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>可观察列表</returns>
    public static ObservableList<T> ToObservableList<T>(this IEnumerable<T> source)
    {
        return new ObservableList<T>(source);
    }

    /// <summary>
    /// 转换至可观察集合
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>可观察集合</returns>
    public static ObservableSet<T> ToObservableSet<T>(this IEnumerable<T> source)
    {
        return new ObservableSet<T>(source);
    }

    /// <summary>
    /// 转换至可观察集合
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TSource">值类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>可观察集合</returns>
    public static ObservableDictionary<TKey, TSource> ToObservableDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector
    )
        where TKey : notnull
    {
        var dic = new ObservableDictionary<TKey, TSource>();
        foreach (var item in source)
        {
            dic.Add(keySelector(item), item);
        }
        return dic;
    }
}
