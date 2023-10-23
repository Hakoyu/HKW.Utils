﻿using HKW.HKWUtils.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.Utils.Tests.Timers;

[TestClass]
public class CountdownTimerTests
{
    [TestMethod]
    public void CountdownTimer()
    {
        var completedCount = 0;
        var stoppedCount = 0;
        var timer = new CountdownTimer();
        timer.AutoReset = true;
        timer.Completed += () =>
        {
            completedCount++;
        };
        timer.Stopped += () =>
        {
            stoppedCount++;
        };
        timer.Start(100);
        Task.Delay(1000).Wait();
        Assert.IsTrue(completedCount == 1);
        Assert.IsTrue(stoppedCount == 0);
        timer.Start(100);
        timer.Stop();
        Assert.IsTrue(completedCount == 1);
        Assert.IsTrue(stoppedCount == 1);
        Task.Delay(100).Wait();
        timer.Continue();
        Task.Delay(1000).Wait();
        Assert.IsTrue(completedCount == 2);
        Assert.IsTrue(stoppedCount == 1);
    }
}
