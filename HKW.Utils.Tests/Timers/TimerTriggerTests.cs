using HKW.HKWUtils.Timers;

namespace HKW.HKWUtils.Tests.Timers;

[TestClass]
public class TimerTriggerTests
{
    [TestMethod]
    public async Task TimeTrigger()
    {
        var triggerCount = 5;
        var timer = new TimerTrigger();
        timer.TimedTrigger += (s, e) =>
        {
            if (e.Count == triggerCount)
            {
                s.Stop();
            }
        };
        timer.Start(100, 100);
        await Task.Delay(1000);
        Assert.IsTrue(timer.State.Count == triggerCount);
    }
}
