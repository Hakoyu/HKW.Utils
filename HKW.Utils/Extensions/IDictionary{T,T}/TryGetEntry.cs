namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试获取键值对
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="pair">键值对</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetPair<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        out KeyValuePair<TKey, TValue> pair
    )
    {
        var result = dictionary.TryGetValue(key, out var value);
        if (result)
            pair = new(key, value!);
        else
            pair = default;
        return result;
    }
}
