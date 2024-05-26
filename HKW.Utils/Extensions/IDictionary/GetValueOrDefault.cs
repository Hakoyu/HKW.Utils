using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取或返回默认值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue? defaultValue = default!
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        if (dictionary.TryGetValue(key, out var value) is false)
            value = defaultValue;
        return value;
    }
}
