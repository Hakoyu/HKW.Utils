using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();
    public static I18nCore I18nCore = new();
    public static I18nResource<string, string> I18nResource =
        new(I18nCore) { DefaultValue = string.Empty, FillDefaultValueForNewCulture = true };

    private static void Main(string[] args)
    {
#if DEBUG
        var p1 = new ObservablePoint<int>();
        //var p2 = new ReadOnlyObservablePoint<int>();
        //var ol = new ObservableList<int>();
        //var l = new List<int>();
        //ol.BindingList(l);
        //ol.Add(1);
        ////ol.AddRange([2, 3, 4]);
        ////ol.RemoveAt(0);
        ////l.RemoveRange(0, 2);
        //////ol.RemoveRange(0, 2);
        ////ol.Clear();
        //ol.BindingList(l, true);
        //ol.Add(1);
        //I18nResource.AddCulture("zh");
        //I18nResource.AddCulture("en");
        //I18nResource.SetCurrentCulture("zh");
        //var v1 = new TestModel();
        //v1.PropertyChangedX += (s, e) =>
        //{
        //    Debug.WriteLine($"V1: {e.PropertyName} = {e.NewValue}");
        //    return;
        //};
        //var v2 = new TestModel();
        //v2.PropertyChangedX += (s, e) =>
        //{
        //    Debug.WriteLine($"V2: {e.PropertyName} = {e.NewValue}");
        //    return;
        //};
        //v1.ID = "1";
        //v1.Name = "zh-1";
        //v1.Name = "zh-2";
        //v1.ID = "2";
        //v2.ID = "1";
        //v2.Name = "zh-2";
        //v2.ID = "1";
        //v2.Name = "zh-11";
        //I18nResource.SetCurrentCulture("en");
        //v1.Name = "en-1";
        //v2.ID = "2";
        //v2.Name = "en-2";
        //v2.ID = "1";
        //v2.Name = "zh-22";
        //I18nResource.SetCurrentCulture("zh");
        //I18nCore.ClearI18nResources();

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
    public TestModel()
    {
        //NotifyPropertyOnPropertyChanged(nameof(ID), nameof(IDX));
        NotifyMemberPropertyChanged(nameof(Size), Size);
        MemberPropertyChangedX += TestModel_MemberPropertyChangedX;
        Size.Width = 10;
        RemoveNotifyMemberPropertyChanged(Size);
        Size.Height = 10;
        //Program.I18nResource.I18nObjectInfos.Add(
        //    this, new(this, SetProperty)
        //);
    }

    private void TestModel_MemberPropertyChangedX(object? sender, MemberPropertyChangedXEventArgs e)
    {
        return;
    }

    #region ID
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _id = string.Empty;

    public string ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    #endregion

    //#region Name
    //public string Name
    //{
    //    get => Program.I18nResource.GetCurrentCultureDataOrDefault(ID, string.Empty);
    //    set => Program.I18nResource.SetCurrentCultureData(ID, value);
    //}
    //#endregion

    public ObservableSize<int> Size { get; } = new();
}
#endif
