﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();

    private static void Main(string[] args)
    {
        var span = "aaa,bbb,,ccc".AsSpan();
        foreach (var str in span.Split(','))
        {
            Console.WriteLine(str.ToString());
        }
    }

    static bool Foo(int i)
    {
        return true;
    }
    //var s = list.First()
    //s.Contains('a');
    //var enumInfo = new ObservableEnum<TestEnum>();
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
#if DEBUG

#endif
}


#if DEBUG

#endif
