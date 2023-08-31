using HKW.HKWUtils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变前接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifySetChanging<T>
{
    /// <summary>
    /// 列表改变前事件
    /// </summary>
    public event XCancelEventHandler<IObservableSet<T>, NotifySetChangingEventArgs<T>>? SetChanging;
}
