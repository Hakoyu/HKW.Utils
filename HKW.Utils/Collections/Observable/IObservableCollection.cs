using System.Collections.Specialized;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 可观察集合接口
/// </summary>
public interface IObservableCollection<T> : ICollection<T>
{
    /// <summary>
    /// 启用清理将触发删除事件
    /// <para>
    /// 启用后 <see cref="ICollection{T}.Clear"/> 方法将触发 <see cref="NotifyCollectionChangedAction.Remove"/> 类型事件, 可以此获取清理的集合
    /// </para>
    /// </summary>
    public bool TriggerRemoveActionOnClear { get; set; }
}
