using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 属性改变接收器
/// </summary>
/// <typeparam name="TSender">发送者类型</typeparam>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void PropertyChangedResponderEventHandler<TSender>(TSender sender, EventArgs e);
