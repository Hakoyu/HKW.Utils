using System.Globalization;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class ObservableCultureDataDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 文化
    /// </summary>
    public CultureInfo Culture { get; internal set; } = null!;
}
