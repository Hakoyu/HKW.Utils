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
        //ObservableDictionary<int, int> dict = new() { ObservableKeysAndValues = true };
        //dict.AddRange(
        //    Enumerable.Range(dict.Count, 10).Select(i => new KeyValuePair<int, int>(i, i))
        //);
        ObservableList<int> list = new();
        list.CollectionChanged += (s, e) =>
        {
            return;
        };
        list.AddRange(Enumerable.Range(0, 10));
        //ObservableDictionary<int, int> dic =
        //    new(Enumerable.Range(0, 10).ToDictionary(i => i, i => i));
        //dic.TriggerRemoveActionOnClear = true;
        //dic.DictionaryChanged += (v) =>
        //{
        //    return;
        //};
        //dic.DictionaryChanging += (v) =>
        //{
        //    return;
        //};
        //dic.CollectionChanged += (s, e) =>
        //{
        //    return;
        //};
        //dic.ChangeRange(dic.Select(p => new KeyValuePair<int, int>(p.Key, 999)));

        //ObservableList<int> list = new();
        //list.ListChanging += (v) =>
        //{
        //    return;
        //};
        //list.ListChanged += (v) =>
        //{
        //    return;
        //};
        //list.CollectionChanged += (s, e) =>
        //{
        //    return;
        //};
        ////for (var i = 0; i < 1000; i++)
        ////    list.Add(i);
        //list.AddRange(Enumerable.Range(0, 1000));
        //ObservableCollection
        //INotifyDictionaryChanging notify = dictionary;
        //notify.DictionaryChanging += (v) =>
        //{
        //    Console.WriteLine(v.Action);
        //};
        //notify.DictionaryChanging += (v) =>
        //{
        //    Console.WriteLine(v.OldPairs);
        //};
        //notify.DictionaryChanging += (v) =>
        //{
        //    Console.WriteLine(v.NewPairs);
        //};
        //_dict.Add("1", "111");
        //Console.WriteLine(_dict.Keys is ICollection);

        //HashSet<int> set2 = Enumerable.Range(5, 10).ToHashSet();
        //Console.WriteLine(string.Join(" ", set1));
        //Console.WriteLine(string.Join(" ", set2));
        //Console.WriteLine();
        //var a = set1.Intersect(set2);
        //var b = set1.Except(set2);
        //var c = set1.Union(set2).Except(set1.Intersect(set2));
        //Console.WriteLine(string.Join(" ", a));
        //Console.WriteLine(string.Join(" ", b));
        //Console.WriteLine(string.Join(" ", c));
        //set1.SymmetricExceptWith(set2);
        //Console.WriteLine(string.Join(" ", set1));

        //ObservableSet<int> set1 =
        //    new(Enumerable.Range(0, 10).ToHashSet()) { NotifySetModifies = true };
        //HashSet<int> set2 = Enumerable.Range(5, 10).ToHashSet();
        //set1.SetChanging += (s, e) =>
        //{
        //    return;
        //};
        //set1.SetChanged += (s, e) =>
        //{
        //    return;
        //};
        //var newCount = 0;
        //var oldCount = 0;
        //set1.CollectionChanged += (s, e) =>
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //        newCount++;
        //    if (e.Action == NotifyCollectionChangedAction.Remove)
        //        oldCount++;
        //};
        //set1.SymmetricExceptWith(set2);
        //Console.WriteLine($"{newCount}, {oldCount}");
        //return;
        //ObservableCollection<int> ints = new(Enumerable.Range(0, 10));
        //ints.CollectionChanged += (s, e) =>
        //{
        //    return;
        //};
        //ints.RemoveAt(0);
        //var timer = new TimerTrigger();
        //var lastTime = 0.0;
        //timer.TimedTrigger += (v) =>
        //{
        //    var temp = v.Elapsed.TotalMilliseconds - lastTime;
        //    Console.WriteLine(
        //        $"Trigger {v.State.Counter} {v.Elapsed.TotalMilliseconds:f4}ms {temp:f4}ms"
        //    );
        //    lastTime += temp;
        //    if (v.State.Counter == 100)
        //        v.Stop();
        //};
        //stopWatch.Start();
        //timer.Start(1000, 30);
        //Task.Delay(5000).Wait();
        //stopWatch.Stop();
        //Console.WriteLine($"\nEnd  {stopWatch.ElapsedMilliseconds:f4}ms");

        //var timer = new CountdownTimer();
        //timer.AutoReset = true;
        //timer.Completed += () =>
        //{
        //    Console.WriteLine("Time up");
        //};
        //timer.Stopped += () =>
        //{
        //    Console.WriteLine("Time stop");
        //};
        //timer.Start(1000);
        //Task.Delay(2000).Wait();
        //Console.WriteLine($"{timer.Elapsed.TotalMilliseconds:f4}ms");
        //timer.Start(1000);
        //Console.WriteLine($"{timer.Elapsed.TotalMilliseconds:f4}ms");
        //timer.Stop();
        //Task.Delay(1000).Wait();
        //Console.WriteLine($"{timer.Elapsed.TotalMilliseconds:f4}ms");
        //timer.Continue();
        //Task.Delay(2000).Wait();
        //Console.WriteLine($"{timer.Elapsed.TotalMilliseconds:f4}ms");
        //Dictionary<int, List<int>> _dic =
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
    }

#if DEBUG

#endif
}

#if DEBUG
#endif
