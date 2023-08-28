using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知集合改变时接口
/// </summary>
/// <typeparam name="T">类型</typeparam>
public interface INotifySetChanging<T>
{
    /// <summary>
    /// 列表改变时事件
    /// </summary>
    public event NotifySetChangingEventHandler<T>? SetChanging;
}
