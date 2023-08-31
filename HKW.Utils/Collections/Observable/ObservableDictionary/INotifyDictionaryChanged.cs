using HKW.HKWUtils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 通知字典已改变接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface INotifyDictionaryChanged<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 字典已改变事件
    /// </summary>
    public event XEventHandler<
        IObservableDictionary<TKey, TValue>,
        NotifyDictionaryChangedEventArgs<TKey, TValue>
    >? DictionaryChanged;
}
