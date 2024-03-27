using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察的I18n资源
/// </summary>
public class I18nResourceInfo : INotifyPropertyChanged
{
    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; private set; }

    /// <summary>
    /// 资源
    /// </summary>
    public object I18nResource { get; private set; }

    /// <inheritdoc/>
    /// <param name="resName">资源名称</param>
    /// <param name="i18nRes">资源</param>
    protected I18nResourceInfo(string resName, object i18nRes)
    {
        ResourceName = resName;
        I18nResource = i18nRes;
    }

    /// <summary>
    /// 刷新资源
    /// </summary>
    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new(null));
    }

    /// <summary>
    /// 属性改变委托
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}
