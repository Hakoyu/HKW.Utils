using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections.Observable.ObservableStack;

/// <summary>
/// 堆栈改变方案
/// </summary>
internal enum NotifyStackChangeAction
{
    Push,
    Peek,
    Pop
}
