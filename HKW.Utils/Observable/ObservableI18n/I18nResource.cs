using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// I18n资源
/// 配合 <see cref="ObservableI18nCore"/> 使用
/// <para>示例:
/// <code><![CDATA[
/// public partial class MainWindowViewModel : ObservableObject
/// {
///     public static ObservableI18nCore I18nCore { get; } = new();
///     [ObservableProperty]
///     public ObservableI18nResource<TestI18nResource> _i18n = I18nCore.Create<TestI18nResource>(new());
/// }
/// public class TestI18nResource : II18nResource
/// {
///     public I18nResource I18nResource { get; } = new(MainWindowViewModel.I18nCore);
///
///     public string Name => GetCultureData(nameof(Name));
/// }
/// ]]>
/// </code>
/// </para>
/// </summary>
public class I18nResource<T>
{
    /// <summary>
    /// 本地化核心
    /// </summary>
    public ObservableI18nCore I18nCore { get; }

    /// <summary>
    /// 严格模式 默认为: <see langword="false"/>
    /// <para>开启后任何失败操作均会触发异常</para>
    /// </summary>
    public bool SrictMode { get; set; } = false;

    /// <summary>
    /// 覆盖模式 <see langword="false"/>
    /// <para>开启后添加已存在的新值覆盖旧值</para>
    /// </summary>
    public bool CanOverride { get; set; } = false;

    #region I18nData
    /// <summary>
    /// 所有文化数据
    /// <para>(Culture.Name, <see cref="CurrentCultureDatas"/>)</para>
    /// </summary>
    public ObservableDictionary<
        CultureInfo,
        ObservableDictionary<string, T>
    > CultureDatas { get; } = new();

    /// <summary>
    /// 当前文化数据
    /// <para>(key, value)</para>
    /// </summary>
    public ObservableDictionary<string, T> CurrentCultureDatas { get; private set; } = new();
    #endregion
    /// <inheritdoc/>
    /// <param name="core">本地化核心</param>
    public I18nResource(ObservableI18nCore core)
    {
        I18nCore = core;
        core.CurrentCultureChanged += (s, e) =>
        {
            SetCurrentCulture(e.CultureInfo);
        };
    }

    /// <inheritdoc/>
    /// <param name="core">本地化核心</param>
    /// <param name="currentCulture">当前文化</param>
    public I18nResource(ObservableI18nCore core, CultureInfo currentCulture)
        : this(core)
    {
        AddCulture(currentCulture);
        SetCurrentCulture(currentCulture);
    }

    /// <inheritdoc/>
    /// <param name="core">本地化核心</param>
    /// <param name="currentCultureName">当前文化名称</param>
    public I18nResource(ObservableI18nCore core, string currentCultureName)
        : this(core, CultureInfo.GetCultureInfo(currentCultureName)) { }

    #region CurrentCultureData Operation
    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void AddCultureData(string key, T value)
    {
        if (CanOverride)
        {
            if (CurrentCultureDatas.TryAdd(key, value) is false)
                CurrentCultureDatas[key] = value;
        }
        if (SrictMode)
        {
            CurrentCultureDatas.Add(key, value);
        }
        CurrentCultureDatas.TryAdd(key, value);
    }

    /// <summary>
    /// 删除文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(string key)
    {
        return CurrentCultureDatas.Remove(key);
    }

    /// <summary>
    /// 清空文化数据
    /// </summary>
    public void ClearCultureData()
    {
        CurrentCultureDatas.Clear();
    }

    /// <summary>
    /// 获取当前文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public T GetCurrentCultureData(string key)
    {
        return CurrentCultureDatas[key];
    }

    /// <summary>
    /// 获取当前文化数据或默认值
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据</returns>
    public T GetCurrentCultureDataOrDefault(
        CultureInfo culture,
        string key,
        T defaultValue = default!
    )
    {
        if (CurrentCultureDatas.TryGetValue(key, out var value))
            return value;
        return defaultValue;
    }

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCurrentCultureData(string key, [MaybeNullWhen(false)] out T value)
    {
        return CurrentCultureDatas.TryGetValue(key, out value);
    }
    #endregion
    #region  CultureData Operation
    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void AddCultureData(CultureInfo culture, string key, T value)
    {
        if (SrictMode)
        {
            if (CanOverride)
            {
                if (CultureDatas[culture].TryAdd(key, value) is false)
                    CultureDatas[culture][key] = value;
            }
            CultureDatas[culture].Add(key, value);
        }
        if (CultureDatas.TryGetValue(culture, out var data))
        {
            if (data.TryAdd(key, value) is false && CanOverride)
                data[key] = value;
        }
        else
        {
            CultureDatas.Add(culture, new() { [key] = value });
        }
    }

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void AddCultureData(string cultureName, string key, T value) =>
        AddCultureData(CultureInfo.GetCultureInfo(cultureName), key, value);

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(CultureInfo culture, string key)
    {
        if (SrictMode)
        {
            CultureDatas[culture].Remove(key);
            return true;
        }
        if (CultureDatas.TryGetValue(culture, out var data))
            return data.Remove(key);
        return false;
    }

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(string cultureName, string key) =>
        RemoveCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 清空文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    public void ClearCultureData(CultureInfo culture)
    {
        if (SrictMode)
        {
            CultureDatas[culture].Clear();
        }
        if (CultureDatas.TryGetValue(culture, out var data))
            data.Clear();
    }

    /// <summary>
    /// 清空文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    public void ClearCultureData(string cultureName) =>
        ClearCultureData(CultureInfo.GetCultureInfo(cultureName));

    /// <summary>
    /// 获取文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <returns>数据</returns>
    public T GetCultureData(CultureInfo culture, string key)
    {
        return CultureDatas[culture][key];
    }

    /// <summary>
    /// 获取文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>数据</returns>
    public T GetCultureData(string cultureName, string key) =>
        GetCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 获取文化数据或默认值
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据</returns>
    public T GetCultureDataOrDefault(CultureInfo culture, string key, T defaultValue = default!)
    {
        if (CultureDatas.TryGetValue(culture, out var data))
        {
            if (data.TryGetValue(key, out var value))
                return value;
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取文化数据或默认值
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据</returns>
    public T GetCultureDataOrDefault(string cultureName, string key, T defaultValue = default!) =>
        GetCultureDataOrDefault(CultureInfo.GetCultureInfo(cultureName), key, defaultValue);

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCultureData(
        CultureInfo culture,
        string key,
        [MaybeNullWhen(false)] out T value
    )
    {
        return CultureDatas[culture].TryGetValue(key, out value);
    }

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCultureData(
        string cultureName,
        string key,
        [MaybeNullWhen(false)] out T value
    ) => TryGetCultureData(CultureInfo.GetCultureInfo(cultureName), key, out value);

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCultureData(
        CultureInfo culture,
        string key,
        [MaybeNullWhen(false)] out T value,
        T defaultValue
    )
    {
        if (CultureDatas.TryGetValue(culture, out var data))
        {
            if (data.TryGetValue(key, out value))
                return true;
        }
        value = defaultValue;
        return false;
    }

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCultureData(
        string cultureName,
        string key,
        [MaybeNullWhen(false)] out T value,
        T defaultValue
    ) => TryGetCultureData(CultureInfo.GetCultureInfo(cultureName), key, out value, defaultValue);
    #endregion

    #region Culture Operation
    /// <summary>
    /// 设置当前文化
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool SetCurrentCulture(CultureInfo culture)
    {
        if (SrictMode)
        {
            CurrentCultureDatas = CultureDatas[culture];
            return true;
        }
        if (CultureDatas.TryGetValue(culture, out var cultureData))
        {
            CurrentCultureDatas = cultureData;
            return true;
        }
        CurrentCultureDatas = null!;
        return false;
    }

    /// <summary>
    /// 设置当前文化
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool SetCurrentCulture(string cultureName) =>
        SetCurrentCulture(CultureInfo.GetCultureInfo(cultureName));

    /// <summary>
    /// 添加文化
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCulture(CultureInfo culture)
    {
        if (SrictMode)
        {
            CultureDatas.Add(culture, new());
            return true;
        }
        return CultureDatas.TryAdd(culture, new());
    }

    /// <summary>
    /// 添加文化
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCulture(string cultureName) =>
        AddCulture(CultureInfo.GetCultureInfo(cultureName));

    /// <summary>
    /// 删除文化
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCulture(CultureInfo culture)
    {
        return CultureDatas.Remove(culture);
    }

    /// <summary>
    /// 删除文化
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCulture(string cultureName) =>
        RemoveCulture(CultureInfo.GetCultureInfo(cultureName));

    /// <summary>
    /// 清空所有文化
    /// <para>注意:
    /// 此操作会将 <see cref="CurrentCultureDatas"/> 设置为 <see langword="null"/>
    /// </para>
    /// </summary>
    public void ClearCulture()
    {
        CultureDatas.Clear();
        CurrentCultureDatas = null!;
    }

    /// <summary>
    /// 替换文化
    /// </summary>
    /// <param name="oldCulture">旧文化</param>
    /// <param name="newCulture">新文化</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool ReplaceCulture(CultureInfo oldCulture, CultureInfo newCulture)
    {
        if (CultureDatas.ContainsKey(oldCulture) is false || CultureDatas.ContainsKey(newCulture))
            return false;
        var cultureData = CultureDatas[oldCulture];
        CultureDatas.Remove(oldCulture);
        return CultureDatas.TryAdd(newCulture, cultureData);
    }

    /// <summary>
    /// 替换文化
    /// </summary>
    /// <param name="oldCultureName">旧文化名称</param>
    /// <param name="newCultureName">新文化名称</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool ReplaceCulture(string oldCultureName, string newCultureName) =>
        ReplaceCulture(
            CultureInfo.GetCultureInfo(oldCultureName),
            CultureInfo.GetCultureInfo(newCultureName)
        );

    #endregion
}
