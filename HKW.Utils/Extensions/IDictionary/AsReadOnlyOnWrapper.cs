using HKW.HKWUtils.Collections;
using System.Collections.ObjectModel;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 创建一个只读字典,可手动转换字典中的值为只读模式
    /// <para>示例:
    /// <code>
    /// <![CDATA[
    /// Dictionary<int, List<int>> dic = new();
    /// ReadOnlyDictionary<int, IReadOnlyCollection<int>> readOnlyDic = dic.AsReadOnlyOnWrapper<int, List<int>, IReadOnlyCollection<int>>();
    ///
    /// Dictionary<int, HashSet<int>> dic = new();
    /// ReadOnlyDictionary<int, IReadOnlySet<int>> readOnlyDic = dic.AsReadOnlyOnWrapper<int, HashSet<int>, IReadOnlySet<int>>();
    ///
    /// Dictionary<int, Dictionary<int,int>> dic = new();
    /// ReadOnlyDictionary<int, IReadOnlyDictionary<int,int>> readOnlyDic = dic.AsReadOnlyOnWrapper<int, Dictionary<int,int>, IReadOnlyDictionary<int,int>>();
    /// ]]>
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    /// <typeparam name="TReadOnlyValue">只读值</typeparam>
    /// <param name="this">此字典</param>
    /// <returns>只读字典</returns>
    public static ReadOnlyDictionary<TKey, TReadOnlyValue> AsReadOnlyOnWrapper<
        TKey,
        TValue,
        TReadOnlyValue
    >(this IDictionary<TKey, TValue> @this)
        where TKey : notnull
        where TValue : TReadOnlyValue
        where TReadOnlyValue : notnull
    {
        return new(new ReadOnlyDictionaryWrapper<TKey, TValue, TReadOnlyValue>(@this));
    }
}
