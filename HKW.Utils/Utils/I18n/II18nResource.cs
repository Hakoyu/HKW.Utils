using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// I18n资源接口
/// </summary>
public interface II18nResource : IDisposable
{
    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// 当前文化, 切换文化会刷新所有I18n资源
    /// </summary>
    public CultureInfo CurrentCulture { get; set; }
}
