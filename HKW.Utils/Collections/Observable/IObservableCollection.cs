using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 非通用可观察集合接口
/// </summary>
public interface IObservableCollection : ICollection
{
    /// <summary>
    /// 启用清理将触发删除事件
    /// <para>
    /// 启用后 <see cref="ICollection{T}.Clear"/> 方法将触发 <see cref="NotifyCollectionChangedAction.Remove"/> 类型事件, 可以此获取清理的集合
    /// </para>
    /// </summary>
    public bool TriggerRemoveActionOnClear { get; set; }

    /// <summary>
    /// 添加多个项目
    /// </summary>
    /// <param name="items">项目</param>
    public void AddRange(IEnumerable items);
}
