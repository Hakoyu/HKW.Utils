﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Drawing;
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

        //var r = FileUtils.Compare("D:\\Downloads\\G2151.7z1", "D:\\Downloads\\G2151.7z1");
        //var text = "Chinese_name".ToPascal('_', sourceToLower: false);
        //var l = new ObservableList<int>();
        //l.AddRange(new[] { 1, 2, 3 });
        //var its = typeof(TestModel1).GetInterfaces();
        //var it = its.LastOrDefault(i => i.Name == typeof(IEnableLogger<>).Name);
        //var enums = Enum.GetValues<TestEnum1>()
        //    .Where(x =>
        //        NumberUtils.CompareX(
        //            x,
        //            0,
        //            Enum.GetUnderlyingType(typeof(TestEnum1)),
        //            ComparisonOperatorType.Inequality
        //        )
        //    )
        //    .ToFrozenSet();
        //var info = new EnumInfo<TestEnum1>(TestEnum1.None | TestEnum1.A);
        //var infos = info.GetFlagInfos();
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

internal partial class TestModel1 : TestModel, IEnableLogger<ReactiveObjectX> { }

internal partial class TestModel : ReactiveObjectX, IEnableLogger<TestModel>
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
internal enum TestEnum1 : Int64
{
    [Display(Name = "None_Name", ShortName = "None_ShortName", Description = "None_Description")]
    None,

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
