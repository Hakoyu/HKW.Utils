using HKW.HKWUtils.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.Utils.Tests.Timers;

[TestClass]
public class TimerTriggerTests
{
    [TestMethod]
    public void TimeTrigger()
    {
        var triggerCount = 5;
        var timer = new TimerTrigger();
        timer.TimedTrigger += (v) =>
        {
            if (v.State.Counter == triggerCount)
            {
                v.Stop();
            }
        };
        timer.Start(100, 100);
        Task.Delay(3000).Wait();
        Assert.AreEqual(timer.State.Counter, triggerCount);
    }
}
