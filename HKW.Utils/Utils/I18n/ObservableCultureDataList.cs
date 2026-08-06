using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class ObservableCultureDataList<TKey, TValue> : ObservableList<TValue>
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; internal set; }

    /// <inheritdoc/>
    public ObservableCultureDataList(TKey key)
    {
        Key = key;
    }
}
