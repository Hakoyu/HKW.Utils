using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class DictionaryExtensions
{
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        /// <summary>
        /// 尝试添加键和值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否添加成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAddX(TKey key, TValue value)
        {
            return dictionary is Dictionary<TKey, TValue> sourceDictionary
                ? sourceDictionary.TryAdd(key, value)
                : dictionary.TryAdd(key, value);
        }

        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>获取的键值对</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue> GetPair(TKey key)
        {
            return new(key, dictionary[key]);
        }

        /// <summary>
        /// 尝试获取键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="pair">键值对</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPair(TKey key, out KeyValuePair<TKey, TValue> pair)
        {
            var result = dictionary.TryGetValue(key, out var value);
            if (result)
                pair = new(key, value!);
            else
                pair = default;
            return result;
        }

        /// <summary>
        /// 按键删除全部对象
        /// </summary>
        /// <param name="keys">键</param>
        public void RemoveAll(IEnumerable<TKey> keys)
        {
            ArgumentNullException.ThrowIfNull(keys);
            foreach (var key in keys)
                dictionary.Remove(key);
        }
    }

    /// <summary>
    /// 获取或自动创建新值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key
    )
        where TKey : notnull
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var value) is false)
            value = dictionary[key] = new();
        return value;
    }

    /// <summary>
    /// 获取或返回新值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">新值</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var oldValue) is false)
            oldValue = dictionary[key] = value;
        return oldValue;
    }

    /// <summary>
    /// 获取或返回新值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="getValue">新值</param>
    /// <returns>值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> getValue
    )
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var oldValue) is false)
            oldValue = dictionary[key] = getValue();
        return oldValue;
    }

    /// <summary>
    /// 尝试获取或创建新值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>调用前是否已存在键; false 也可能表示已成功创建新值.</returns>
    /// <remarks>
    /// 查找和写入分开执行, 不提供并发原子 GetOrAdd 语义.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value
    )
        where TKey : notnull
        where TValue : new()
    {
        var result = dictionary.TryGetValue(key, out value!);
        if (result is false)
            value = dictionary[key] = new();
        return result;
    }

    /// <summary>
    /// 尝试获取或创建值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="newValue">新值</param>
    /// <returns>调用前是否已存在键; false 也可能表示已成功创建新值.</returns>
    /// <remarks>
    /// 查找和写入分开执行, 不提供并发原子 GetOrAdd 语义.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value,
        TValue newValue
    )
        where TKey : notnull
    {
        var result = dictionary.TryGetValue(key, out value!);
        if (result is false)
            value = dictionary[key] = newValue;
        return result;
    }

    /// <summary>
    /// 尝试获取或创建值, 新值会被添加到字典中
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="getValue">新值</param>
    /// <returns>调用前是否已存在键; false 也可能表示已成功创建新值.</returns>
    /// <remarks>
    /// 查找和写入分开执行, 不提供并发原子 GetOrAdd 语义.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValueOrCreate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out TValue value,
        Func<TValue> getValue
    )
        where TKey : notnull
    {
        var result = dictionary.TryGetValue(key, out value!);
        if (result is false)
            value = dictionary[key] = getValue();
        return result;
    }
}
