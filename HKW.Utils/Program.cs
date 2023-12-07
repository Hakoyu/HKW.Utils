using HKW.HKWUtils.Observable;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();

    private static void Main(string[] args)
    {
        //stopWatch.Start();
        //CountdownTimer timer = new();
        //timer.Completed += () =>
        //{
        //    Console.WriteLine($"Completed {stopWatch.ElapsedMilliseconds:f4}");
        //    return;
        //};
        //timer.Start(1000);
        //Task.Delay(500).Wait();
        //timer.Stop();
        //timer.Continue();
        //Task.Delay(1000).Wait();
    }

#if DEBUG

#endif
}

#if DEBUG
public class TestViewModel : ObservableClass<TestViewModel>
{
    int _value0 = 0;
    public int Value0
    {
        get => _value0;
        set => SetProperty(ref _value0, value);
    }
}
#endif
