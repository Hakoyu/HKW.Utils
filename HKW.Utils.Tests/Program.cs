using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtilsTests.Collections;
using HKW.HKWUtilsTests.Extensions;
using HKW.HKWUtilsTests.Observable;
using HKW.HKWUtilsTests.Utils;
using ReactiveUI;
using Splat;

namespace HKW;

#pragma warning disable S1144,S2223,S1643,S3626,S2342,S1481
internal class Program
{
    private static void Main(string[] args)
    {
#if !Release
        //new UndoableListTests().ListTest();
        //ObservableChangeSet<int> ints = new();

        //MemoizingMRUCache
        //var dic = new Dictionary<string, object>();
        //Console.WriteLine(((ICollection<object>)dic.Values).IsReadOnly);
        //try
        //{
        //    new CyclicEnumeratorTests().List();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //}
#endif
    }
}
