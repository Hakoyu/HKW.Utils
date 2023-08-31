using HKW.HKWUtils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合已改变接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifySetChanged<T>
{
    /// <summary>
    /// 列表已改变事件
    /// </summary>
    public event XEventHandler<IObservableSet<T>, NotifySetChangedEventArgs<T>>? SetChanged;
}
