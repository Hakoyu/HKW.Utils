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
    /// 当前文化, 切换文化会刷新所有I18n资源. null 表示无当前文化, 不自动选择其他文化.
    /// </summary>
    public CultureInfo? CurrentCulture { get; set; }

    /// <summary>
    /// 所有文化
    /// </summary>
    public ICollection<CultureInfo> Cultures { get; }
}
