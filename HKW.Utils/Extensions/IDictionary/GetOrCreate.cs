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
    /// 获取或创建值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key
    )
        where TKey : notnull
        where TValue : new()
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        if (dictionary.TryGetValue(key, out var value) is false)
            value = dictionary[key] = new();
        return value;
    }

    /// <summary>
    /// 获取或返回默认值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">默认值</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? GetOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        if (dictionary.TryGetValue(key, out var oldValue) is false)
            oldValue = dictionary[key] = value;
        return oldValue;
    }

    /// <summary>
    /// 获取或创建值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="createValue">创建值</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> createValue
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        ArgumentNullException.ThrowIfNull(createValue, nameof(createValue));
        if (dictionary.TryGetValue(key, out var value) is false)
            value = dictionary[key] = createValue();
        return value;
    }
}
