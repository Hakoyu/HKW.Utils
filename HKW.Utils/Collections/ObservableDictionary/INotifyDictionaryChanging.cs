using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典改变时接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface INotifyDictionaryChanging<TKey, TValue>
{
    /// <summary>
    /// 字典改变时事件
    /// </summary>
    public event NotifyDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;
}
