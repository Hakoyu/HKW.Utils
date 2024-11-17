using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取键值对
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <returns>获取的键值对</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KeyValuePair<TKey, TValue> GetPair<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key
    )
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));
        return new(key, dictionary[key]);
    }
}
