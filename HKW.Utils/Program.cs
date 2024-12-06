using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW;

internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();
    public static I18nCore I18nCore = new();
    public static I18nResource<string, string> I18nResource =
        new(I18nCore) { DefaultValue = string.Empty, FillDefaultValueToData = true };
    public IntegratedReadOnlyList<int, List<int>, ReadOnlyCollection<int>> List { get; } =
        new(new(), l => new(l));
    public ReadOnlyCollection<int> ReadOnlyList => List.ReadOnlyList;
    public static Point<int> point { get; set; } = new(1, 2);
    public static Point point1 { get; set; } = new(1, 2);

    private static void Main(string[] args)
    {
#if !Release
        var dic = new ObservableSelectableDictionary<int, int>(
            new Dictionary<int, int>()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3
            },
            1
        );
        dic.WhenValueChanged(x => x.SelectedItem)
            .Subscribe(x =>
            {
                Console.WriteLine(x);
            });
        dic[1] = 10;
        //var observableDictionary = new ObservableDictionaryWrapper<
        //    string,
        //    string,
        //    Dictionary<string, string>
        //>(new Dictionary<string, string>());
        //var e = TestEnum1.A | TestEnum1.B | TestEnum1.C;
        //var e1 = e.RemoveFlag(TestEnum1.B);
        //EnumInfo<TestEnum1>.DefaultToString = x => $"{x.Value}_String";
        //var info = EnumInfo<TestEnum1>.GetInfo(e);
        //var str = info.ToString();
        //var i = info.Names.GetItemByIndex(3);
        //var leader = new ObservableSelectionGroupLeader();
        //var members = Enumerable
        //    .Range(0, 2)
        //    .Select(_ => new ObservableSelectionGroupMember() { IsSelected = false })
        //    .ToObservableList();
        //var group = new ObservableSelectionGroup<
        //    ObservableSelectionGroupLeader,
        //    ObservableSelectionGroupMember,
        //    ObservableList<ObservableSelectionGroupMember>
        //>(
        //    new(
        //        leader,
        //        nameof(ObservableSelectionGroupLeader.IsSelected),
        //        x => x.IsSelected,
        //        (x, v) => x.IsSelected = v
        //    ),
        //    new(
        //        new(),
        //        nameof(ObservableSelectionGroupMember.IsSelected),
        //        x => x.IsSelected,
        //        (x, v) => x.IsSelected = v
        //    ),
        //    members
        //);
        //var group = new ObservableSelectionGroup<TestModel>();
        //group.Add(new(new() { ID = "A" }));
        //group.Add(new(new() { ID = "B" }));
        ////group.Add(new(new() { ID = "C" }));
        //group.First().IsSelected = true;
        //group.Last().IsSelected = true;
        //var r = group.Leader.Value;
#endif
    }

    private static void T_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine($"PropertyName: {e.PropertyName}");
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
#if !Release

#endif
}

#if !Release

internal static class TestExtensions
{
    public static bool TryGetResult<T>(this T? value, [MaybeNullWhen(false)] out T result)
    {
        result = value;
        if (result is null)
            return false;
        return true;
    }
}

internal partial class TestModel : ReactiveObjectX
{
    public TestModel()
    {
        //this.WhenAnyValue(x => x.CanExecute)
        //    .Buffer(2, 1)
        //    .Select(b => (Previous: b[0], Current: b[1]))
        //    .Subscribe(pair =>
        //    {
        //        var oldValue = pair.Previous;
        //        var newValue = pair.Current;
        //        Console.WriteLine($"ExampleProperty 的值已经改变，旧的值是：{oldValue}，新的值是：{newValue}");
        //    });
        //CanExecute = true;
        //Program.I18nResource.I18nObjects.Add(new(this));
        //var i18nObject = Program.I18nResource.I18nObjects.Last();
        //i18nObject.AddProperty(nameof(ID), x => ((TestModel)x).ID, nameof(Name), true);
    }

    public EnumInfo<TestEnum1> Enum { get; set; } = EnumInfo<TestEnum1>.GetInfo(TestEnum1.A);

    [ReactiveProperty]
    public string ID { get; set; } = string.Empty;

    [ReactiveI18nProperty("Program.I18nResource", "I18nObject", nameof(ID), true)]
    public string Name
    {
        get => Program.I18nResource.GetCurrentCultureDataOrDefault(ID);
        set => Program.I18nResource.SetCurrentCultureData(ID, value);
    }

    [NotifyPropertyChangeFrom("")]
    public I18nObject<string, string> I18nObject => new(this);

    [ReactiveProperty]
    public bool CanExecute { get; set; }

    [ReactiveCommand(CanExecute = nameof(CanExecute))]
    public void Test()
    {
        Console.WriteLine(nameof(Test));
    }

    [ReactiveCommand]
    public async Task Test1Async()
    {
        await Task.Delay(1000);
        Console.WriteLine(nameof(Test1Async));
    }
}

[Flags]
internal enum TestEnum1
{
    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1 << 0,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 1 << 1,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 1 << 2,
}

internal enum TestEnum2
{
    A,
    B,
    C,
}
#endif
