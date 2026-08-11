namespace HKW.HKWUtils.Collections;

/// <summary>
/// 字典包装器接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
#pragma warning disable S2436
public interface IDictionaryWrapper<TKey, TValue, TDictionary>
    : ICollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary>
#pragma warning restore S2436
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <summary>
    /// 原始字典
    /// </summary>
    protected TDictionary SourceDictionary { get; }
}
