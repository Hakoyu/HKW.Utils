using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知接收器
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void NotifyReceivedEventHandler(ICommand sender, CancelEventArgs e);
