using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知列表改变时接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifyListChanging<T>
{
    /// <summary>
    /// 列表改变时事件
    /// </summary>
    public event NotifyListChangingEventHandler<T>? ListChanging;
}
