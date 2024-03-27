namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察本地化资源实例
/// </summary>
/// <typeparam name="TResource">I18n资源</typeparam>
public class I18nResourceInfo<TResource> : I18nResourceInfo
    where TResource : class
{
    /// <summary>
    /// 本地化资源
    /// </summary>
    public new TResource I18nResource => (TResource)base.I18nResource;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="i18nResource">本地化资源</param>
    public I18nResourceInfo(TResource i18nResource)
        : base(i18nResource.GetType().FullName!, i18nResource) { }
}
