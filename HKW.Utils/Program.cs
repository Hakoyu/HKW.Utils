using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
        new(I18nCore) { DefaultValue = string.Empty, FillDefaultValueToNewCulture = true };
    public IntegratedReadOnlyList<int, List<int>, ReadOnlyCollection<int>> List { get; } =
        new(new(), l => new(l));
    public ReadOnlyCollection<int> ReadOnlyList => List.ReadOnlyList;

    private static void Main(string[] args)
    {
#if !Release
        //var size = new Size<int>("114, 514");
        //I18nResource.AddCulture("zh");
        //I18nResource.AddCulture("en");
        //I18nResource.AddCultureData("zh", "Name", "Name-CN");
        //I18nResource.AddCultureData("en", "Name", "Name-EN");
        //I18nResource.SetCurrentCulture("zh");
        //var t = new TestModel();
        //t.PropertyChanged += T_PropertyChanged;
        //t.ID = "Name";
        //Console.WriteLine(t.Name);
        //I18nResource.SetCurrentCulture("en");
        //Console.WriteLine(t.Name);
        //t.ID = "Name2";
        //Console.WriteLine(t.Name);
        //t.CanExecute = !t.CanExecute;
        //var c = t.TestCommand as ICommand;
        //c.CanExecuteChanged += C_CanExecuteChanged;
        //c.CanExecute(null);
        //c.Execute(null);
        //var command = ReactiveCommand.Create(() => { });
        //var e1 = EnumInfo<TestEnum1>.GetInfo(TestEnum1.A);
        //e1.GetName = (v) => $"{v.Value}_1";
        //var e2 = EnumInfo<TestEnum2>.GetInfo(TestEnum2.A);
        //var n1 = e1.Name;
        //var n2 = e2.Name;
        var list = new ObservableSelectableList<int, List<int>>(new() { 1, 2, 3, 4, 5 });
        list.SelectedIndex = 0;
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

    [I18nProperty("Program.I18nResource", nameof(ID), true)]
    public string Name
    {
        get => Program.I18nResource.GetCurrentCultureDataOrDefault(ID);
        set => Program.I18nResource.SetCurrentCultureData(ID, value);
    }

    [ReactiveProperty]
    [NotifyPropertyChangedFrom(nameof(ID))]
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
    A,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C,
}

internal enum TestEnum2
{
    A,
    B,
    C,
}
#endif
