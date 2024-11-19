using System.Collections.Concurrent;
using System.Globalization;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class ObservableCultureDataDictionary<TKey, TValue>
    : ObservableDictionaryWrapper<CultureInfo, TValue, ConcurrentDictionary<CultureInfo, TValue>>,
        ICloneable<ObservableCultureDataDictionary<TKey, TValue>>
    where TKey : notnull
{
    /// <inheritdoc/>
    public ObservableCultureDataDictionary()
        : base(new()) { }

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

    /// <summary>
    /// 值克隆行动,如果值类型是引用类型可设置此行动
    /// </summary>
    public Func<TValue, TValue>? ValueCloneAction { get; set; }

    #region ICloneable
    /// <inheritdoc/>
    public ObservableCultureDataDictionary<TKey, TValue> Clone()
    {
        var dictionary = new ObservableCultureDataDictionary<TKey, TValue>() { Key = Key };
        foreach (var pair in this)
        {
            dictionary.Add(
                pair.Key,
                ValueCloneAction is null ? pair.Value : ValueCloneAction(pair.Value)
            );
        }
        return dictionary;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
    #endregion
}
