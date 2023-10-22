using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Timers;
using System.Collections;
using System.Collections.ObjectModel;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();

    private static void Main(string[] args)
    {
        ObservableCollection<int> oc = new();
        ObservableList<int> ints = new();
        ints.Add(1);
        oc.Add(1);
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
#endif
