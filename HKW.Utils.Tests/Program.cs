using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtilsTests.Collections;
using HKW.HKWUtilsTests.Extensions;
using HKW.HKWUtilsTests.Observable;
using HKW.HKWUtilsTests.Utils;

namespace HKW.HKWUtilsTests;

internal class Program
{
    public static CultureInfo CultureCN { get; } = CultureInfo.GetCultureInfo("cn");
    public static CultureInfo CultureEN { get; } = CultureInfo.GetCultureInfo("en");

    private static void Main(string[] args)
    {
        var vm = new TestViewModel();
        var i18nRes = vm.I18nResource;
        vm.PropertyChanged += Vm_PropertyChanged;
        vm.ID = "114";
        vm.Name = "Name_CN";
        vm.ID = "514";
        //i18nRes.RenameKey("114", "514");
        // source.PropertyName
        // source.GetPropertyValue()
        // Pro

        //i18nRes.Observable.Action
    }

    private static void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var propertyInfo = typeof(TestViewModel).GetProperty(e.PropertyName!);
        Console.WriteLine($"{e.PropertyName}: {propertyInfo?.GetValue(sender)}");
    }
}

public class TestViewModel : INotifyPropertyChanged
{
    public TestViewModel()
    {
        I18nResource!.AddCulture(Program.CultureEN);
    }

    /// <summary>
    /// ID
    /// </summary>
    public string ID
    {
        get => field;
        set
        {
            if (EqualityComparer<string>.Default.Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(nameof(ID));
            OnPropertyChanged(nameof(DescriptionID));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
        }
    }

    /// <summary>
    /// 描述ID
    /// </summary>
    public string DescriptionID => $"{ID}_Description";

    /// <summary>
    /// 名称
    /// </summary>
    public string Name
    {
        get => I18nResource.GetDataOrDefault(ID, this);
        set => I18nResource.SetData(ID, value);
    }

    /// <summary>
    /// 详情
    /// </summary>
    public string Description
    {
        get => I18nResource.GetDataOrDefault(DescriptionID);
        set =>
            I18nResource
                .SetData(DescriptionID, value)
                .Action(() => OnPropertyChanged(nameof(Description)), null);
    }

    /// <summary>
    /// I18n资源
    /// </summary>
    public ObservableI18nResource<string, string> I18nResource =>
        field ??= new(nameof(TestViewModel), Program.CultureCN) { GetDefaultValue = (s, _) => s };

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
