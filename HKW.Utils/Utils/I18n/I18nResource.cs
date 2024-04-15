using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// I18n资源
/// 配合 <see cref="I18nCore"/> 使用
/// <para>示例:
/// <code><![CDATA[
/// public partial class MainWindowViewModel : ObservableObject
/// {
///     public static I18nCore I18nCore { get; } = new();
///     [ObservableProperty]
///     public I18nResource<TestI18nResource> _i18n = I18nCore.Create<TestI18nResource>(new());
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
public class I18nResource<TKey, TValue> : II18nResource, INotifyPropertyChanged
    where TKey : notnull
{
    /// <inheritdoc/>
    public I18nResource()
    {
        Cultures.SetChanged += Cultures_SetChanged;
        CultureDatas.DictionaryChanged += CultureDatas_DictionaryChanged;
        I18nObjectInfos.ListChanged += I18nObjectInfos_ListChanged;
    }

    /// <inheritdoc/>
    /// <param name="core">I18n核心</param>
    /// <param name="addCurrentCulture">为资源添加当前文化</param>
    public I18nResource(I18nCore core, bool addCurrentCulture = false)
        : this()
    {
        I18nCore = core;
        if (addCurrentCulture)
        {
            AddCulture(core.CurrentCulture);
            SetCurrentCulture(core.CurrentCulture);
        }
    }

    /// <inheritdoc/>
    /// <param name="core">本地化核心</param>
    /// <param name="culture">文化</param>
    public I18nResource(I18nCore core, CultureInfo culture)
        : this()
    {
        I18nCore = core;
        AddCulture(culture);
        SetCurrentCulture(culture);
    }

    /// <inheritdoc/>
    /// <param name="core">本地化核心</param>
    /// <param name="cultureName">文化名称</param>
    public I18nResource(I18nCore core, string cultureName)
        : this(core, CultureInfo.GetCultureInfo(cultureName)) { }

    #region I18nCore
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private I18nCore? _i18nCore = null!;

    /// <summary>
    /// 本地化核心
    /// </summary>
    public I18nCore? I18nCore
    {
        get => _i18nCore;
        set
        {
            if (_i18nCore is not null)
            {
                _i18nCore.CurrentCultureChanged -= Core_CurrentCultureChanged;
                _i18nCore.I18nResources.Remove(this);
            }
            _i18nCore = value;
            if (_i18nCore is not null)
            {
                _i18nCore.CurrentCultureChanged -= Core_CurrentCultureChanged;
                _i18nCore.CurrentCultureChanged += Core_CurrentCultureChanged;
                _i18nCore.I18nResources.Add(this);
            }
        }
    }
    #endregion

    #region CurrentCulture
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private CultureInfo _currentCulture = null!;

    /// <summary>
    /// 当前文化
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (Cultures.Contains(value) is false)
                return;
            _currentCulture = value;
            OnPropertyChanged(nameof(CurrentCulture));
            RefreshAllI18nObject();
        }
    }
    #endregion

    /// <summary>
    /// 所有文化
    /// </summary>
    public ObservableSet<CultureInfo> Cultures { get; } = new();

    #region I18nDatas
    /// <summary>
    /// 所有文化数据
    /// <para>
    /// (<see langword="TKey"/>, (<see cref="CultureInfo"/>, <see langword="TValue"/>))
    /// </para>
    /// </summary>
    public ObservableDictionary<
        TKey,
        ObservableCultureDataDictionary<TKey, TValue>
    > CultureDatas { get; } = new();

    #endregion
    private void Cultures_SetChanged(
        IObservableSet<CultureInfo> sender,
        NotifySetChangedEventArgs<CultureInfo> e
    )
    {
        if (e.Action is SetChangeAction.Clear)
        {
            CurrentCulture = null!;
            foreach (var datas in CultureDatas.Values)
                datas.Clear();
        }
        else
        {
            if (e.OldItems is not null)
            {
                foreach (var item in e.OldItems)
                    ClearCultureData(item);
            }
        }
    }

    private void Core_CurrentCultureChanged(I18nCore sender, CultureChangedEventArgs e)
    {
        SetCurrentCulture(e.CultureInfo);
    }

    private void CultureDatas_DictionaryChanged(
        IObservableDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>> sender,
        NotifyDictionaryChangedEventArgs<TKey, ObservableCultureDataDictionary<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            newPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
            newPair.Value.DictionaryChanged += CurrentCultureDatas_DictionaryChanged;
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var oldPair) is false)
                return;
            oldPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetNewPair(out var newPair))
            {
                newPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
                newPair.Value.DictionaryChanged += CurrentCultureDatas_DictionaryChanged;
            }
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
            }
        }
    }

    private void CurrentCultureDatas_DictionaryChanged(
        IObservableDictionary<CultureInfo, TValue> sender,
        NotifyDictionaryChangedEventArgs<CultureInfo, TValue> e
    )
    {
        if (sender is not ObservableCultureDataDictionary<TKey, TValue> cultureDatas)
            return;
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            CultureDataChanged?.Invoke(
                this,
                new(
                    newPair.Key,
                    cultureDatas.Key,
                    e.OldPair is null ? default : e.OldPair.Value.Value,
                    newPair.Value
                )
            );
            foreach (var source in I18nObjectInfos)
            {
                if (source.TargetPropertyNamesWithKey.ContainsKey(cultureDatas.Key))
                    source.NotifyPropertyChangedWithKey(cultureDatas.Key);
            }
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var oldPair) is false)
                return;
            CultureDataChanged?.Invoke(
                this,
                new(oldPair.Key, cultureDatas.Key, oldPair.Value, default)
            );

            foreach (var source in I18nObjectInfos)
            {
                if (source.TargetPropertyNamesWithKey.ContainsKey(cultureDatas.Key))
                    source.NotifyPropertyChangedWithKey(cultureDatas.Key);
            }
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            if (e.TryGetOldPair(out var oldPair) is false)
                return;
            CultureDataChanged?.Invoke(
                this,
                new(oldPair.Key, cultureDatas.Key, oldPair.Value, newPair.Value)
            );
            foreach (var source in I18nObjectInfos)
            {
                if (source.TargetPropertyNamesWithKey.ContainsKey(cultureDatas.Key))
                    source.NotifyPropertyChangedWithKey(cultureDatas.Key);
            }
        }
    }

    private void I18nObjectInfos_ListChanged(
        IObservableList<I18nObjectInfo<TKey>> sender,
        NotifyListChangedEventArgs<I18nObjectInfo<TKey>> e
    )
    {
        if (e.Action is ListChangeAction.Add)
        {
            if (e.NewItems is null)
                return;
            foreach (var item in e.NewItems)
            {
                item.KeyChanged -= Item_KeyChanged;
                item.KeyChanged += Item_KeyChanged;
            }
        }
        else if (e.Action is ListChangeAction.Remove)
        {
            if (e.OldItems is null)
                return;
            foreach (var item in e.OldItems)
            {
                item.KeyChanged -= Item_KeyChanged;
            }
        }
        else if (e.Action is ListChangeAction.Replace)
        {
            if (e.OldItems is null)
                return;
            foreach (var item in e.OldItems)
            {
                item.KeyChanged -= Item_KeyChanged;
            }
            if (e.NewItems is null)
                return;
            foreach (var item in e.NewItems)
            {
                item.KeyChanged -= Item_KeyChanged;
                item.KeyChanged += Item_KeyChanged;
            }
        }
    }

    private void Item_KeyChanged(I18nObjectInfo<TKey> sender, (TKey OldKey, TKey NewKey) e)
    {
        if (CultureDatas.TryGetValue(e.OldKey, out var oldDatas) is false)
        {
            CultureDatas.TryAdd(e.NewKey, new() { Key = e.NewKey });
            return;
        }
        if (oldDatas.HasValue() is false)
        {
            // 如果旧值不存在数据,尝试新键新值
            CultureDatas.TryAdd(e.NewKey, new() { Key = e.NewKey });
            // 如果在所有引用中未被使用,则删除
            if (
                I18nObjectInfos.All(i =>
                    i.TargetPropertyNamesWithKey.ContainsKey(e.OldKey) is false
                )
            )
                CultureDatas.Remove(e.OldKey);
            return;
        }
        if (CultureDatas.TryGetValue(e.NewKey, out var newDatas) is false)
        {
            // 如果新值不存在数据
            newDatas = new() { Key = e.NewKey };
            // 先添加, 否则不会注册事件
            CultureDatas.TryAdd(e.NewKey, newDatas);
            newDatas.AddRange(oldDatas);
            // 如果在所有引用中未被使用,则删除
            if (
                I18nObjectInfos.All(i =>
                    i.TargetPropertyNamesWithKey.ContainsKey(e.OldKey) is false
                )
            )
                CultureDatas.Remove(e.OldKey);
        }
        else
        {
            // 如果新值存在数据
            foreach (var data in oldDatas)
            {
                if (data.Value is not null)
                {
                    // 如果为null则不替换
                    if (data.Value is not string str || string.IsNullOrWhiteSpace(str) is false)
                        newDatas[data.Key] = data.Value;
                }
            }
        }
    }

    #region CurrentCultureData
    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCurrentCultureData(TKey key, TValue value)
    {
        if (CultureDatas.TryGetValue(key, out var datas) is false)
            datas = CultureDatas[key] = new() { Key = key };
        return datas.TryAdd(CurrentCulture, value);
    }

    /// <summary>
    /// 设置或覆盖文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void SetCurrentCultureData(TKey key, TValue value)
    {
        if (AddCurrentCultureData(key, value) is false)
            CultureDatas[key][CurrentCulture] = value;
    }

    /// <summary>
    /// 删除文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCurrentCultureData(TKey key)
    {
        return CultureDatas[key].Remove(CurrentCulture);
    }

    /// <summary>
    /// 清空文化数据
    /// </summary>
    public void ClearCurrentCultureData()
    {
        ClearCultureData(CurrentCulture);
    }

    /// <summary>
    /// 获取当前文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public TValue GetCurrentCultureData(TKey key)
    {
        return CultureDatas[key][CurrentCulture];
    }

    /// <summary>
    /// 获取当前文化数据或默认值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据</returns>
    public TValue GetCurrentCultureDataOrDefault(TKey key, TValue defaultValue = default!)
    {
        if (CultureDatas[key].TryGetValue(CurrentCulture, out var value))
            return value;
        return defaultValue;
    }

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCurrentCultureData(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return CultureDatas[key].TryGetValue(CurrentCulture, out value);
    }
    #endregion

    #region  CultureData
    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCultureData(CultureInfo culture, TKey key, TValue value)
    {
        if (CultureDatas.TryGetValue(key, out var datas))
        {
            return datas.TryAdd(culture, value);
        }
        else
        {
            CultureDatas.Add(key, new() { Key = key, [culture] = value });
            return true;
        }
    }

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCultureData(string cultureName, TKey key, TValue value) =>
        AddCultureData(CultureInfo.GetCultureInfo(cultureName), key, value);

    /// <summary>
    /// 添加多个文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="items">项目 (键, 值)</param>
    public void AddCultureDatas(CultureInfo culture, IEnumerable<(TKey Key, TValue Value)> items)
    {
        foreach (var item in items)
            AddCultureData(culture, item.Key, item.Value);
    }

    /// <summary>
    /// 添加多个文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="items">项目 (键, 值)</param>
    public void AddCultureDatas(string cultureName, IEnumerable<(TKey Key, TValue Value)> items)
    {
        foreach (var item in items)
            AddCultureData(CultureInfo.GetCultureInfo(cultureName), item.Key, item.Value);
    }

    /// <summary>
    /// 设置或覆盖文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void SetCultureData(CultureInfo culture, TKey key, TValue value)
    {
        if (CultureDatas.TryGetValue(key, out var data))
        {
            data[culture] = value;
        }
        else
        {
            CultureDatas.Add(key, new() { [culture] = value });
        }
    }

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void SetCultureData(string cultureName, TKey key, TValue value) =>
        SetCultureData(CultureInfo.GetCultureInfo(cultureName), key, value);

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(CultureInfo culture, TKey key)
    {
        if (CultureDatas.TryGetValue(key, out var data))
            return data.Remove(culture);
        return false;
    }

    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(string cultureName, TKey key) =>
        RemoveCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 清空文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    public void ClearCultureData(CultureInfo culture)
    {
        foreach (var data in CultureDatas.Values)
            data.Remove(culture);
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
    public TValue GetCultureData(CultureInfo culture, TKey key)
    {
        return CultureDatas[key][culture];
    }

    /// <summary>
    /// 获取文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>数据</returns>
    public TValue GetCultureData(string cultureName, TKey key) =>
        GetCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 获取文化数据或默认值
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据</returns>
    public TValue GetCultureDataOrDefault(
        CultureInfo culture,
        TKey key,
        TValue defaultValue = default!
    )
    {
        if (CultureDatas.TryGetValue(key, out var data))
        {
            if (data.TryGetValue(culture, out var value))
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
    public TValue GetCultureDataOrDefault(
        string cultureName,
        TKey key,
        TValue defaultValue = default!
    ) => GetCultureDataOrDefault(CultureInfo.GetCultureInfo(cultureName), key, defaultValue);

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCultureData(
        CultureInfo culture,
        TKey key,
        [MaybeNullWhen(false)] out TValue value
    )
    {
        return CultureDatas[key].TryGetValue(culture, out value);
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
        TKey key,
        [MaybeNullWhen(false)] out TValue value
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
        TKey key,
        [MaybeNullWhen(false)] out TValue value,
        TValue defaultValue
    )
    {
        if (CultureDatas.TryGetValue(key, out var data))
        {
            if (data.TryGetValue(culture, out value))
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
        TKey key,
        [MaybeNullWhen(false)] out TValue value,
        TValue defaultValue
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
        if (Cultures.Contains(culture) is false)
            return false;
        _currentCulture = culture;
        OnPropertyChanged(nameof(CurrentCulture));
        RefreshAllI18nObject();
        return true;
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
        return Cultures.Add(culture);
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
        return Cultures.Remove(culture);
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
    /// <para>
    /// 注意: 此操作会将 <see cref="CurrentCulture"/> 设置为 <see langword="null"/>
    /// </para>
    /// </summary>
    public void ClearCulture()
    {
        CultureDatas.Clear();
    }

    /// <summary>
    /// 替换文化
    /// </summary>
    /// <param name="oldCulture">旧文化</param>
    /// <param name="newCulture">新文化</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool ReplaceCulture(CultureInfo oldCulture, CultureInfo newCulture)
    {
        if (Cultures.Contains(oldCulture) is false || Cultures.Contains(newCulture))
            return false;
        foreach (var datas in CultureDatas.Values)
        {
            if (datas.Remove(oldCulture, out var data))
                datas[newCulture] = data;
        }
        Cultures.Remove(oldCulture);
        Cultures.Add(newCulture);
        return true;
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
    /// <summary>
    /// I18n对象信息
    /// <para>
    /// (INotifyPropertyChangedX, I18nObjectInfo)
    /// </para>
    /// </summary>
    public ObservableList<I18nObjectInfo<TKey>> I18nObjectInfos { get; } = new();

    ///// <summary>
    ///// 注册通知
    ///// </summary>
    ///// <param name="source">源</param>
    ///// <param name="onPropertyChanged">属性改变行动</param>
    ///// <param name="propertyDatas">属性数据</param>
    //public I18nObjectInfo<TKey> RegisterNotify(
    //    INotifyPropertyChangedX source,
    //    Action<string> onPropertyChanged,
    //    IEnumerable<(
    //        string KeyPropertyName,
    //        TKey KeyValue,
    //        IEnumerable<string> TargetPropertyNames
    //    )> propertyDatas
    //)
    //{
    //    if (I18nObjectInfos.TryGetValue(source, out var info) is false)
    //        info = I18nObjectInfos[source] = new I18nObjectInfo<TKey>(
    //            source,
    //            onPropertyChanged,
    //            propertyDatas
    //        );
    //    return info;
    //}

    /// <summary>
    /// 刷新所有I18nObject
    /// </summary>
    public void RefreshAllI18nObject()
    {
        foreach (var info in I18nObjectInfos)
            info.NotifyAllPropertyChanged();
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>
    /// 文化数据改变后事件
    /// </summary>
    public event CultureDataChangedEventHandler<TKey, TValue>? CultureDataChanged;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}
