﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Timers;

/// <summary>
/// 倒计时完成事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void CountdownEventHandler(CountdownTimer sender, EventArgs e);
