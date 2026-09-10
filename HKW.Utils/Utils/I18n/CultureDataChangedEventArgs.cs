using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 通知文化数据改变后事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class CultureDataChangedEventArgs<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="cultureInfo">文化信息</param>
    /// <param name="key">键</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    public CultureDataChangedEventArgs(
        TKey key,
        TValue? oldValue,
        TValue? newValue,
        CultureInfo? cultureInfo
    )
    {
        CultureInfo = cultureInfo;
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// 文化信息
    /// </summary>
    public CultureInfo? CultureInfo { get; }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// 旧值
    /// </summary>
    public TValue? OldValue { get; }

    /// <summary>
    /// 新值
    /// </summary>
    public TValue? NewValue { get; }
}
