using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 成员属性改变后事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void MemberPropertyChangedXEventHandler(
    object? sender,
    MemberPropertyChangedXEventArgs e
);
