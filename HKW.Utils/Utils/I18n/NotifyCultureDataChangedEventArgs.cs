using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 通知文化数据改变后事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="cultureInfo">文化信息</param>
/// <param name="key">键</param>
/// <param name="oldValue">旧值</param>
/// <param name="newValue">新值</param>
public class NotifyCultureDataChangedEventArgs<TKey, TValue>(
    CultureInfo cultureInfo,
    TKey key,
    TValue? oldValue,
    TValue? newValue
)
    where TKey : notnull
{
    /// <summary>
    /// 文化信息
    /// </summary>
    public CultureInfo CultureInfo { get; } = cultureInfo;

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; } = key;

    /// <summary>
    /// 旧值
    /// </summary>
    public TValue? OldValue { get; } = oldValue;

    /// <summary>
    /// 新值
    /// </summary>
    public TValue? NewValue { get; } = newValue;
}
