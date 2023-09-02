using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试获取条目
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="entry">条目</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetEntry<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [NotNullWhen(true)] out KeyValuePair<TKey, TValue>? entry
    )
    {
        var result = dictionary.TryGetValue(key, out var value);
        if (result)
            entry = new(key, value!);
        else
            entry = null;
        return result;
    }
}
