using System.Buffers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;
using ReactiveUI.Builder;

namespace HKW;

#pragma warning disable S1144,S2223,S1643,S3626,S2342,S1481
internal class Program
{
    private static System.Diagnostics.Stopwatch stopWatch = new();
    public static CultureInfo[] Cultures { get; } =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures);

    //public static I18nCore I18nCore = new();
    public static ObservableI18nResource<string, string> I18nResource { get; } =
        new("Main", Cultures, Cultures.First());
    public static CultureInfo CultureEN => field ??= CultureInfo.GetCultureInfo("en-us");
    public static CultureInfo CultureCN => field ??= CultureInfo.CurrentCulture;

    //public IntegratedReadOnlyList<int, List<int>, ReadOnlyCollection<int>> List { get; } =
    //    new(new(), l => new(l));
    //public ReadOnlyCollection<int> ReadOnlyList => List.ReadOnlyList;
    public static Point<int> point { get; set; } = new(1, 2);
    public static Point point1 { get; set; } = new(1, 2);

    private static void Main(string[] args)
    {
#if !Release
        RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        try
        {
            var resource = I18nResource;
            foreach (var c in resource.Cultures)
                resource.SetDatas(
                    Enumerable
                        .Range(0, 10)
                        .Select(x => KeyValuePair.Create(x.ToString(), $"{x}_{c.Name}")),
                    c
                );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
#endif
    }

    private static void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TestModel model)
            return;
        if (e.PropertyName == "Name")
        {
            Console.WriteLine(model.Name);
        }
    }

    private static void ATimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
    }

    private static void Timer_TimedTrigger(object? sender, EventArgs e)
    {
        Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
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
        Program.I18nResource.Observable.Action(
            this,
            x => ((TestModel)x).ID,
            (x, _) =>
            {
                x.As<TestModel>().RaisePropertyChanged(nameof(Name));
            }
        );
        this.WhenAnyValue(x => x.Name)
            .Buffer(2, 1)
            .Select(b => (Previous: b[0], Current: b[1]))
            .Subscribe(pair =>
            {
                var oldValue = pair.Previous;
                var newValue = pair.Current;
                Console.WriteLine($"Name 的值已经改变，旧的值是：{oldValue}，新的值是：{newValue}");
            });
        //CanExecute = true;
        //Program.I18nResource.I18nObjects.Add(new(this));
        //var i18nObject = Program.I18nResource.I18nObjects.Last();
        //i18nObject.AddProperty(nameof(ID), x => ((TestModel)x).ID, nameof(Name), true);
    }

    public EnumInfo<TestEnum1> Enum { get; set; } = EnumInfo<TestEnum1>.GetInfo(TestEnum1.A);

    [ReactiveProperty]
    public string ID { get; set; } = string.Empty;

    partial void OnIDChanged(string oldValue, string newValue)
    {
        this.RaisePropertyChanged(nameof(Name));
    }

    public string Name
    {
        get => Program.I18nResource.GetDataOrDefault(ID);
        set => Program.I18nResource.SetData(ID, value);
    }

    //[NotifyPropertyChangeFrom("")]
    //public I18nObject<string, string> I18nObject => new(this);

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
