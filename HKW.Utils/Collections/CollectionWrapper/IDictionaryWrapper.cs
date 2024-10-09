namespace HKW.HKWUtils;

/// <summary>
/// 字典包装器接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
public interface IDictionaryWrapper<TKey, TValue, TDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <summary>
    /// 基础字典
    /// </summary>
    public TDictionary BaseDictionary { get; }
}
