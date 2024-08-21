using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试获取或创建值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>获取成功为 <see langword="true"/> 获取失败并创建为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value
    )
        where TKey : notnull
        where TValue : new()
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        var result = dictionary.TryGetValue(key, out value!);
        if (result is false)
            value = dictionary[key] = new();
        return result;
    }

    /// <summary>
    /// 尝试获取或创建值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="createValue">创建值</param>
    /// <returns>获取成功为 <see langword="true"/> 获取失败并创建为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value,
        Func<TValue> createValue
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(createValue, nameof(createValue));
        var result = dictionary.TryGetValue(key, out value!);
        if (result is false)
            value = dictionary[key] = createValue();
        return result;
    }
}
