using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();

    public static ObservableI18nCore I18nCore { get; } = new();

    private static void Main(string[] args)
    {
#if DEBUG

        var test = new TestModel();
        test.PropertyChanged += Test_PropertyChanged;
        var oo = (ObservableObjectX)test;
        oo.PropertyChanged -= Test_PropertyChanged;
#endif
    }

    private static void Test_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        return;
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
internal class TestModel : ObservableObjectX
{
    #region ID
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _id = string.Empty;

    public string ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    #endregion
}
#endif
