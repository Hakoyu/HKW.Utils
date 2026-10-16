using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Key = {Key}, Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableCultureDataList<TKey, TValue> : ObservableListWrapper<TValue, List<TValue>>
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key
    {
        get => field;
        internal set
        {
            field = value;
            OnPropertyChanged(PropertyChangedArgsCache.Key);
        }
    }

    /// <inheritdoc/>
    public ObservableCultureDataList(TKey key)
        : base(new())
    {
        Key = key;
    }
}
