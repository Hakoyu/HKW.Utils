using System.Collections.ObjectModel;
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
        //var test = new TestModel();
        //Console.WriteLine(test.Name);
        //I18nCore.CurrentCulture = CultureInfo.GetCultureInfo("en");
        //Console.WriteLine(test.Name);
        //res.I18nResource.
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
internal class TestModel : ObservableObjectX<TestModel>, II18nResource<string>
{
    public I18nResource<string> I18nResource { get; } =
        new(Program.I18nCore, Program.I18nCore.CurrentCulture);

    public TestModel()
    {
        I18nResource.AddCultureData("zh-CN", nameof(Name), "名字");
        I18nResource.AddCultureData("en", nameof(Name), "Name1");
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

    public string Name => I18nResource.GetCurrentCultureData(nameof(Name));
}
#endif
