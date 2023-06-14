using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取条目
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <returns>获取的键值对</returns>
    public static KeyValuePair<TKey, TValue> GetEntry<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key
    )
    {
        return new(key, dictionary[key]);
    }
}
