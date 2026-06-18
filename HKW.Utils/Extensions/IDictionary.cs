using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        ///// <summary>
        ///// 获取或创建值, 新值会被添加到字典中
        ///// </summary>
        ///// <param name="key">键</param>
        ///// <returns>值</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public TValue GetOrCreate<TNewValue>(TKey key)
        //    where TNewValue : TValue, new()
        //{
        //    if (dictionary.TryGetValue(key, out var value) is false)
        //        value = dictionary[key] = new TNewValue();
        //    return value;
        //}

        ///// <summary>
        ///// 获取或创建值, 新值会被添加到字典中
        ///// </summary>
        ///// <param name="key">键</param>
        ///// <param name="value">默认值</param>
        ///// <returns>值</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public TValue GetOrCreate(TKey key, TValue value)
        //{
        //    if (dictionary.TryGetValue(key, out var oldValue) is false)
        //        oldValue = dictionary[key] = value;
        //    return oldValue;
        //}

        ///// <summary>
        ///// 尝试获取或创建值
        ///// </summary>
        ///// <param name="key">键</param>
        ///// <param name="value">值</param>
        ///// <returns>获取成功为 <see langword="true"/> 失败并创建为 <see langword="false"/></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public bool TryGetValueOrCreate<TNewValue>(TKey key, out TValue value)
        //    where TNewValue : TValue, new()
        //{
        //    var result = dictionary.TryGetValue(key, out value!);
        //    if (result is false)
        //        value = dictionary[key] = new TNewValue();
        //    return result;
        //}

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
        /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
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
    }

    /// <summary>
    /// 获取或创建值, 新值会被添加到字典中
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
    /// 获取或返回默认值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">默认值</param>
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
    /// 尝试获取或创建值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>获取成功为 <see langword="true"/> 失败并创建为 <see langword="false"/></returns>
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
    /// 尝试获取或创建值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="newValue">新值</param>
    /// <returns>获取成功为 <see langword="true"/> 失败并创建为 <see langword="false"/></returns>
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
    /// <param name="dictionary">此字典</param>
    /// <returns>只读字典</returns>
    public static ReadOnlyDictionary<TKey, TReadOnlyValue> AsReadOnlyOnWrapper<
        TKey,
        TValue,
        TReadOnlyValue
    >(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : TReadOnlyValue
        where TReadOnlyValue : notnull
    {
        return new(new ReadOnlyDictionaryWrapper<TKey, TValue, TReadOnlyValue>(dictionary));
    }
}
