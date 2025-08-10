using HKW.HKWUtils.Timers;

namespace HKW.HKWUtils.Tests.Timers;

[TestClass]
public class CountdownTimerTests
{
    [TestMethod]
    public async Task CountdownTimer()
    {
        var completedCount = 0;
        var stoppedCount = 0;
        var timer = new CountdownTimer();
        timer.AutoReset = true;
        timer.Completed += (s, e) =>
        {
            completedCount++;
        };
        timer.Stopped += (s, e) =>
        {
            stoppedCount++;
        };
        timer.Start(100);
        await Task.Delay(500);
        Assert.AreEqual(1, completedCount);
        Assert.AreEqual(0, stoppedCount);
        timer.Start(100);
        timer.Stop();
        Assert.AreEqual(1, completedCount);
        Assert.AreEqual(1, stoppedCount);
        await Task.Delay(100);
        timer.Continue();
        await Task.Delay(500);
        Assert.AreEqual(2, completedCount);
        Assert.AreEqual(1, stoppedCount);
    }
}
