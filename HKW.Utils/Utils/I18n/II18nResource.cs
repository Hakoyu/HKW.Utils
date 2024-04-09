namespace HKW.HKWUtils;

/// <summary>
/// I18n资源接口
/// </summary>
public interface II18nResource
{
    /// <summary>
    /// I18n核心
    /// </summary>
    public I18nCore? I18nCore { get; set; }

    /// <summary>
    /// 刷新所有I18n对象
    /// </summary>
    public void RefreshAllI18nObject();
}
