using HKW.HKWUtils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表已改变接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifyListChanged<T>
{
    /// <summary>
    /// 列表已改变事件
    /// </summary>
    public event XEventHandler<IObservableList<T>, NotifyListChangedEventArgs<T>>? ListChanged;
}
