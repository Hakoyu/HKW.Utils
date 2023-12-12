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
        Assert.IsTrue(completedCount == 1);
        Assert.IsTrue(stoppedCount == 0);
        timer.Start(100);
        timer.Stop();
        Assert.IsTrue(completedCount == 1);
        Assert.IsTrue(stoppedCount == 1);
        await Task.Delay(100);
        timer.Continue();
        await Task.Delay(500);
        Assert.IsTrue(completedCount == 2);
        Assert.IsTrue(stoppedCount == 1);
    }
}
