#if DEBUG
using System;
using System.Collections.Generic;
using System.Buffers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.CollectionUtils;
using HKW.HKWUtils.Utils;
using System.Threading;
#endif

namespace HKW;

internal class Program
{
    private static void Main(string[] args)
    {
#if DEBUG
        var timer = new CountdownTimer(new TimeSpan(0, 0, 1));
        timer.TimeUp += () =>
        {
            Console.WriteLine("Time up");
        };
        timer.TimeStop += () =>
        {
            Console.WriteLine("Time stop");
        };
        timer.Start();
        Thread.Sleep(2000);
        timer.Start(1000);
        timer.Stop();
        timer.Start(1000);
        timer.Stop();
        timer.Start(1000);
        timer.Stop();
        timer.Start(2000);
        //Thread.Sleep(1000);
        //timer.Start(1);
        Thread.Sleep(10000);
        //Dictionary<int, List<int>> sr_dic =
        //    new()
        //    {
        //        [0] = new() { 0 },
        //        [1] = new() { 1 },
        //        [2] = new() { 2 },
        //        [3] = new() { 3 },
        //        [4] = new() { 4 },
        //        [5] = new() { 5 },
        //        [6] = new() { 6 },
        //        [7] = new() { 7 },
        //        [8] = new() { 8 },
        //        [9] = new() { 9 },
        //    };
        //ObservableCollection<int> ints = new();
        //ints.CollectionChanged += (s, e) =>
        //{
        //    if (s is null) { }
        //};
        //ints.Clear();
        //List<List<int>> ll = Enumerable
        //    .Range(0, 100)
        //    .Select(i => new List<int>(Enumerable.Range(0, 11)))
        //    .ToList();
        //Collection<List<int>> cc =
        //    new(
        //        Enumerable
        //            .Range(0, 100)
        //            .Select(i => new List<int>(Enumerable.Range(0, 10).ToList()))
        //            .ToList()
        //    );
        //var result1 = ll.SequenceEqual(cc);
        //var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => new List<int> { x });
        //var readOnlyDictionary = new ReadOnlyDictionary<int, IReadOnlyCollection<int>>(
        //    dic.ToDictionary(x => x.Key, x => (IReadOnlyCollection<int>)x.Value)
        //);
        //var readOnlyDictionaryOnWrapper = dic.AsReadOnlyOnWrapper<
        //    int,
        //    List<int>,
        //    IReadOnlyCollection<int>
        //>();
        //var set = new HashSet<int>(new int[] { 1, 2, 3 });
        //set.IntersectWith(new int[] { 1, 2, 3, 4, 5, 6 });
        //set.ExceptWith(new int[] { 4, 5, 6 });
        //set.SymmetricExceptWith(new int[] { 3, 2, 1, 4, 5, 6 });
        //set.UnionWith(new int[] { 4, 5, 6 });
        //var set = new ObservableSet<int>();
        //var readOnlySet = set.AsReadOnly();
#endif
    }

#if DEBUG

#endif
}

#if DEBUG
#endif
