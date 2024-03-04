using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using HKW.HKWUtils.Observable;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();

    private static void Main(string[] args)
    {
        var enumInfo = new ObservableEnum<TestEnum>();
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
[Flags]
internal enum TestEnum
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z
}
#endif
