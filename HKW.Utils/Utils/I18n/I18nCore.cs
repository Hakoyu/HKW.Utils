using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 本地化核心
/// </summary>
public sealed class I18nCore : INotifyPropertyChanged
{
    private static readonly Lazy<I18nCore> _lazy = new(() =>
        new I18nCore(CultureInfo.CurrentCulture)
    );

    /// <summary>
    /// 默认 I18nCore
    /// </summary>
    public static I18nCore Default => _lazy.Value;

    /// <inheritdoc/>
    /// <param name="cultureInfo">文化信息</param>
    public I18nCore(CultureInfo? cultureInfo = null)
    {
        CurrentCulture = cultureInfo ?? CultureInfo.CurrentCulture;
        I18nResources = new(_i18nResources);
    }

    /// <summary>
    /// 同步改变的文化类型
    /// </summary>
    [DefaultValue(CultureChangeTypes.None)]
    public CultureChangeTypes CultureChangeTypes { get; set; }

    /// <summary>
    /// 本地化资源字典
    /// <para>(ResourceName, I18nResourceInfo)</para>
    /// </summary>
    private readonly Dictionary<string, II18nResource> _i18nResources = [];

    /// <summary>
    /// 本地化资源字典
    /// <para>(ResourceName, I18nResourceInfo)</para>
    /// </summary>
    public ReadOnlyDictionary<string, II18nResource> I18nResources { get; }

    /// <summary>
    /// 当前文化, 切换文化会同步改变所有I18n资源的文化
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => field;
        set
        {
            if (field == value)
                return;
            field = value;

            if (CultureChangeTypes.HasFlag(CultureChangeTypes.ThreadCurrentCulture))
                Thread.CurrentThread.CurrentCulture = value;
            if (CultureChangeTypes.HasFlag(CultureChangeTypes.ThreadCurrentUICulture))
                Thread.CurrentThread.CurrentUICulture = value;
            if (CultureChangeTypes.HasFlag(CultureChangeTypes.CultureInfoCurrentCulture))
                CultureInfo.CurrentCulture = value;
            if (CultureChangeTypes.HasFlag(CultureChangeTypes.CultureInfoCurrentUICulture))
                CultureInfo.CurrentUICulture = value;
            foreach (var pair in _i18nResources)
                pair.Value.CurrentCulture = CurrentCulture;

            CurrentCultureChanged?.Invoke(this, value);
            PropertyChanged?.Invoke(this, new(nameof(CurrentCulture)));
        }
    }

    /// <summary>
    /// 文化改变后事件
    /// </summary>
    public event EventHandler<CultureInfo>? CurrentCultureChanged;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 添加资源
    /// </summary>
    /// <param name="resource">资源</param>
    /// <returns>是否成功添加</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resource"/> 为 <see langword="null"/></exception>
    public bool AddResource(II18nResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        var result = _i18nResources.TryAdd(resource.ResourceName, resource);
        if (result)
            resource.CurrentCulture = CurrentCulture;
        return result;
    }

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="resource">资源</param>
    /// <returns>是否成功删除</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resource"/> 为 <see langword="null"/></exception>
    public bool RemoveResource(II18nResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return _i18nResources.Remove(resource.ResourceName);
    }

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="resourceName">资源名称</param>
    /// <returns>是否成功删除</returns>
    public bool RemoveResource(string resourceName)
    {
        return _i18nResources.Remove(resourceName);
    }

    /// <summary>
    /// 清空资源
    /// </summary>
    public void ClearResource()
    {
        _i18nResources.Clear();
    }
}

/// <summary>
/// 文化改变类型
/// </summary>
[Flags]
public enum CultureChangeTypes
{
    /// <summary>
    /// 不改变
    /// </summary>
    None = 0,

    /// <summary>
    /// <see cref="Thread.CurrentCulture"/>
    /// </summary>
    ThreadCurrentCulture = 1 << 0,

    /// <summary>
    /// <see cref="Thread.CurrentUICulture"/>
    /// </summary>
    ThreadCurrentUICulture = 1 << 1,

    /// <summary>
    /// <see cref="CultureInfo.CurrentCulture"/>
    /// </summary>
    CultureInfoCurrentCulture = 1 << 2,

    /// <summary>
    /// <see cref="CultureInfo.CurrentUICulture"/>
    /// </summary>
    CultureInfoCurrentUICulture = 1 << 3,
}
