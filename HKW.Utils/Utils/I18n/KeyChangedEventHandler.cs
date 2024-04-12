using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Utils;

/// <summary>
/// 键改变后事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void KeyChangedEventHandler<TKey>(
    I18nObjectInfo<TKey> sender,
    (TKey OldKey, TKey NewKey) e
)
    where TKey : notnull;
