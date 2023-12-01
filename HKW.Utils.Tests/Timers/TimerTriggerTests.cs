using HKW.HKWUtils.Timers;

namespace HKWTests.Timers;

[TestClass]
public class TimerTriggerTests
{
    [TestMethod]
    public void TimeTrigger()
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
        Task.Delay(3000).Wait();
        Assert.IsTrue(timer.State.Count == triggerCount);
    }
}
