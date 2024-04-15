using System.Globalization;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class ObservableCultureDataDictionary<TKey, TValue>
    : ObservableDictionary<CultureInfo, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; internal set; } = default!;

    /// <summary>
    /// 通过文化名称获取值
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <returns>值</returns>
    public TValue this[string cultureName]
    {
        get => this[CultureInfo.GetCultureInfo(cultureName)];
        set => this[CultureInfo.GetCultureInfo(cultureName)] = value;
    }
}
