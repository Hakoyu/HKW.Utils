using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using Splat;

namespace HKW.HKWUtils;

/// <summary>
/// I18n资源
/// 配合 <see cref="I18nCore"/> 使用
/// <para>示例:
/// <code><![CDATA[
/// public partial class MainWindowViewModel : ObservableObject
/// {
///     public static I18nCore I18nCore { get; } = new();
///     public I18nResource<TestI18nResource> I18nResource { get; } = I18nCore.Create<TestI18nResource>(new());
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
        CultureDatas.DictionaryChanging += CultureDatas_DictionaryChanging;
        CultureDatas.DictionaryChanged += CultureDatas_DictionaryChanged;
        I18nObjects.SetChanging += I18nObjectInfos_SetChanging;
        I18nObjects.SetChanged += I18nObjectInfos_SetChanged;
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

    /// <summary>
    /// 为新文化填写默认值
    /// <para>
    /// 在添加新文化时,会向文化数据添加 <see cref="DefaultValue"/>
    /// </para>
    /// </summary>
    public bool FillDefaultValueToNewCulture { get; set; } = false;

    /// <summary>
    /// 默认值
    /// </summary>
    public TValue DefaultValue { get; set; } = default!;

    private Func<TValue, TValue>? _valueCloneAction;

    /// <summary>
    /// 值克隆行动,如果值类型是引用类型可设置此行动
    /// </summary>
    public Func<TValue, TValue>? ValueCloneAction
    {
        get => _valueCloneAction ?? DefaultValueCloneAction;
        set => _valueCloneAction ??= value;
    }

    /// <summary>
    /// 默认值克隆行动,如果值类型是引用类型可设置此行动
    /// </summary>
    public static Func<TValue, TValue>? DefaultValueCloneAction { get; set; }

    /// <summary>
    /// 日志记录器
    /// </summary>
    public IEnableLogger? Logger { get; set; }

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
    public ObservableDictionaryWrapper<
        TKey,
        ObservableCultureDataDictionary<TKey, TValue>,
        ConcurrentDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>>
    > CultureDatas { get; } = new(new());

    /// <summary>
    /// I18n对象信息
    /// <para>
    /// (INotifyPropertyChangedX, I18nObjectInfo)
    /// </para>
    /// </summary>
    public ObservableSet<I18nObject<TKey, TValue>> I18nObjects { get; } = new();
    #endregion
    private void Cultures_SetChanged(
        IObservableSet<CultureInfo> sender,
        NotifySetChangeEventArgs<CultureInfo> e
    )
    {
        if (e.Action is SetChangeAction.Add)
        {
            if (e.NewItems is null)
                return;
            if (FillDefaultValueToNewCulture)
            {
                foreach (var item in e.NewItems)
                {
                    foreach (var datas in CultureDatas.Values)
                        datas.TryAdd(item, DefaultValue);
                }
            }
        }
        else if (e.Action is SetChangeAction.Clear)
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

    private void CultureDatas_DictionaryChanging(
        IObservableDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataDictionary<TKey, TValue>> e
    )
    {
        if (sender is not ObservableCultureDataDictionary<TKey, TValue> cultureDatas)
            return;
        if (e.Action is DictionaryChangeAction.Clear)
        {
            foreach (var pair in CultureDatas)
            {
                pair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
            }
        }
    }

    private void CultureDatas_DictionaryChanged(
        IObservableDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataDictionary<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            newPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
            newPair.Value.DictionaryChanged += CurrentCultureDatas_DictionaryChanged;
            newPair.Value.ValueCloneAction = ValueCloneAction;
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
                newPair.Value.ValueCloneAction = ValueCloneAction;
            }
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.DictionaryChanged -= CurrentCultureDatas_DictionaryChanged;
            }
        }
    }

    private void CurrentCultureDatas_DictionaryChanged(
        IObservableDictionary<CultureInfo, TValue> sender,
        NotifyDictionaryChangeEventArgs<CultureInfo, TValue> e
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
                new(newPair.Key, cultureDatas.Key, default, newPair.Value)
            );
            if (_keyChanging)
                return;
            foreach (var obj in I18nObjects)
            {
                if (obj.KeyToTargetNames.ContainsKey(cultureDatas.Key) is false)
                    continue;
                obj.NotifyPropertyChangedByKey(cultureDatas.Key);
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
            if (_keyChanging)
                return;
            foreach (var obj in I18nObjects)
            {
                if (obj.KeyToTargetNames.ContainsKey(cultureDatas.Key) is false)
                    continue;
                obj.NotifyPropertyChangedByKey(cultureDatas.Key);
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
            if (_keyChanging)
                return;
            foreach (var obj in I18nObjects)
            {
                if (obj.KeyToTargetNames.ContainsKey(cultureDatas.Key) is false)
                    continue;
                obj.NotifyPropertyChangedByKey(cultureDatas.Key);
            }
        }
    }

    private void I18nObjectInfos_SetChanging(
        IObservableSet<I18nObject<TKey, TValue>> sender,
        NotifySetChangeEventArgs<I18nObject<TKey, TValue>> e
    )
    {
        if (e.Action is SetChangeAction.Clear)
        {
            foreach (var item in sender)
            {
                item.KeyChanged -= Item_KeyChanged;
            }
        }
    }

    private void I18nObjectInfos_SetChanged(
        IObservableSet<I18nObject<TKey, TValue>> sender,
        NotifySetChangeEventArgs<I18nObject<TKey, TValue>> e
    )
    {
        if (e.Action is SetChangeAction.Add)
        {
            if (e.NewItems is null)
                return;
            foreach (var item in e.NewItems)
            {
                foreach (var key in item.KeyToTargetNames)
                    AddCultureData(key.Key);
                item.KeyChanged -= Item_KeyChanged;
                item.KeyChanged += Item_KeyChanged;
            }
        }
        else if (e.Action is SetChangeAction.Remove)
        {
            if (e.OldItems is null)
                return;
            foreach (var item in e.OldItems)
            {
                item.KeyChanged -= Item_KeyChanged;
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _keyChanging;

    private void Item_KeyChanged(I18nObject<TKey, TValue> sender, (TKey OldKey, TKey NewKey) e)
    {
        // 如果没有旧值则直接添加新值
        if (CultureDatas.TryGetValue(e.OldKey, out var oldDatas) is false)
        {
            CultureDatas.TryAdd(e.NewKey, new() { Key = e.NewKey });
            return;
        }

        // 如果旧值不存在数据,尝试新键新值
        if (oldDatas.HasValue() is false)
        {
            CultureDatas.TryAdd(e.NewKey, new() { Key = e.NewKey });
            // 如果未被使用,则删除
            if (I18nObjects.All(i => i.KeyToTargetNames.ContainsKey(e.OldKey) is false))
                CultureDatas.Remove(e.OldKey);
            return;
        }
        _keyChanging = true;
        // 如果新值不存在数据
        if (CultureDatas.TryGetValue(e.NewKey, out var newDatas) is false)
        {
            newDatas = new() { Key = e.NewKey };
            // 添加(注册事件)
            CultureDatas.TryAdd(e.NewKey, newDatas);
            // 添加并触发事件
            newDatas.AddRange(oldDatas);
            // 如果未被使用,则删除
            if (I18nObjects.All(i => i.KeyToTargetNames.ContainsKey(e.OldKey) is false))
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
        // 手动发送通知,以防重复触发
        foreach (var obj in I18nObjects)
        {
            if (obj.KeyToTargetNames.ContainsKey(e.NewKey) is false)
                continue;
            obj.NotifyPropertyChangedByKey(e.NewKey);
        }
        _keyChanging = false;
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
            datas = CultureDatas[key] = new() { Key = key, ValueCloneAction = ValueCloneAction };
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
    /// <returns>文化数据</returns>
    public TValue GetCurrentCultureDataOrDefault(TKey key, TValue defaultValue = default!)
    {
        if (CultureDatas.TryGetValue(key, out var datas))
        {
            if (datas.TryGetValue(CurrentCulture, out var value))
                return value;
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取当前文化数据或 <see cref="DefaultValue"/>
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public TValue GetCurrentCultureDataOrDefault(TKey key) =>
        GetCurrentCultureDataOrDefault(key, DefaultValue);

    /// <summary>
    /// 尝试获取当前文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool TryGetCurrentCultureData(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (CultureDatas.TryGetValue(key, out var datas))
            return datas.TryGetValue(CurrentCulture, out value);
        value = default;
        return false;
    }
    #endregion

    #region  CultureData
    /// <summary>
    /// 添加文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool AddCultureData(TKey key)
    {
        return CultureDatas.TryAdd(key, new() { Key = key, ValueCloneAction = ValueCloneAction });
    }

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
            CultureDatas.Add(
                key,
                new()
                {
                    Key = key,
                    [culture] = value,
                    ValueCloneAction = ValueCloneAction
                }
            );
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
    /// <param name="datas">项目 (键, 值)</param>
    public void AddCultureDatas(CultureInfo culture, IEnumerable<KeyValuePair<TKey, TValue>> datas)
    {
        foreach (var item in datas)
            AddCultureData(culture, item.Key, item.Value);
    }

    /// <summary>
    /// 添加多个文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="datas">项目 (键, 值)</param>
    public void AddCultureDatas(
        string cultureName,
        IEnumerable<KeyValuePair<TKey, TValue>> datas
    ) => AddCultureDatas(CultureInfo.GetCultureInfo(cultureName), datas);

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
            CultureDatas.Add(
                key,
                new()
                {
                    Key = key,
                    [culture] = value,
                    ValueCloneAction = ValueCloneAction
                }
            );
        }
    }

    /// <summary>
    /// 设置或覆盖文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void SetCultureData(string cultureName, TKey key, TValue value) =>
        SetCultureData(CultureInfo.GetCultureInfo(cultureName), key, value);

    /// <summary>
    /// 设置或覆盖文化数据
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="datas">文化数据</param>
    public void SetCultureDatas(CultureInfo culture, IEnumerable<KeyValuePair<TKey, TValue>> datas)
    {
        foreach (var pair in datas)
            SetCultureData(culture, pair.Key, pair.Value);
    }

    /// <summary>
    /// 设置或覆盖文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="datas">文化数据</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public void SetCultureDatas(
        string cultureName,
        IEnumerable<KeyValuePair<TKey, TValue>> datas
    ) => SetCultureDatas(CultureInfo.GetCultureInfo(cultureName), datas);

    /// <summary>
    /// 删除文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(TKey key)
    {
        return CultureDatas.Remove(key);
    }

    /// <summary>
    /// 删除文化数据
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
    /// 删除文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool RemoveCultureData(string cultureName, TKey key) =>
        RemoveCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 替换键名
    /// </summary>
    /// <param name="oldKey">旧键</param>
    /// <param name="newKey">新键</param>
    /// <param name="override">如果已经存在新键,则强制覆盖</param>
    /// <param name="removeOldData">如果旧键存在则删除旧键</param>
    /// <returns>成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public bool ReplaceCultureDataKey(
        TKey oldKey,
        TKey newKey,
        bool @override = false,
        bool removeOldData = true
    )
    {
        if (CultureDatas.TryGetValue(oldKey, out var data) is false)
            return false;
        if (@override)
        {
            data.Key = newKey;
            CultureDatas[newKey] = removeOldData ? data : data.Clone();
        }
        else
        {
            if (CultureDatas.TryAdd(newKey, data) is false)
                return false;
            data.Key = newKey;
        }
        if (removeOldData)
            CultureDatas.Remove(oldKey);
        return true;
    }

    /// <summary>
    /// 清空文化数据
    /// </summary>
    public void ClearCultureData()
    {
        foreach (var cultureData in CultureDatas)
        {
            cultureData.Value.Clear();
        }
        CultureDatas.Clear();
    }

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
    /// <returns>文化数据</returns>
    public TValue GetCultureData(CultureInfo culture, TKey key)
    {
        return CultureDatas[key][culture];
    }

    /// <summary>
    /// 获取文化数据
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public TValue GetCultureData(string cultureName, TKey key) =>
        GetCultureData(CultureInfo.GetCultureInfo(cultureName), key);

    /// <summary>
    /// 获取文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public IDictionary<CultureInfo, TValue> GetCultureDatas(TKey key)
    {
        return CultureDatas[key];
    }

    /// <summary>
    /// 尝试获取文化数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureDatas">文化数据</param>
    /// <returns>文化数据</returns>
    public bool TryGetCultureDatas(
        TKey key,
        [MaybeNullWhen(false)] out IDictionary<CultureInfo, TValue> cultureDatas
    )
    {
        cultureDatas = null;
        if (CultureDatas.TryGetValue(key, out var datas))
        {
            cultureDatas = datas;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取文化数据复制品
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public IDictionary<CultureInfo, TValue> GetCultureDatasReplica(TKey key)
    {
        return CultureDatas[key].Clone();
    }

    /// <summary>
    /// 尝试获取文化数据复制品
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureDatas">文化数据</param>
    /// <returns>文化数据</returns>
    public bool TryGetCultureDatasReplica(
        TKey key,
        [MaybeNullWhen(false)] out IDictionary<CultureInfo, TValue> cultureDatas
    )
    {
        cultureDatas = null;
        if (CultureDatas.TryGetValue(key, out var datas))
        {
            cultureDatas = datas.Clone();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取文化数据或默认值
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>文化数据</returns>
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
    /// 获取文化数据或 <see cref="DefaultValue"/>
    /// </summary>
    /// <param name="culture">文化</param>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public TValue GetCultureDataOrDefault(CultureInfo culture, TKey key) =>
        GetCultureDataOrDefault(culture, key, DefaultValue);

    /// <summary>
    /// 获取文化数据或默认值
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>文化数据</returns>
    public TValue GetCultureDataOrDefault(
        string cultureName,
        TKey key,
        TValue defaultValue = default!
    ) => GetCultureDataOrDefault(CultureInfo.GetCultureInfo(cultureName), key, defaultValue);

    /// <summary>
    /// 获取文化数据或 <see cref="DefaultValue"/>
    /// </summary>
    /// <param name="cultureName">文化名称</param>
    /// <param name="key">键</param>
    /// <returns>文化数据</returns>
    public TValue GetCultureDataOrDefault(string cultureName, TKey key) =>
        GetCultureDataOrDefault(cultureName, key, DefaultValue);

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

    /// <summary>
    /// 为所有空值填充默认值
    /// </summary>
    /// <param name="defaultValue">默认值</param>
    public void FillDefaultValue(TValue defaultValue)
    {
        foreach (var datas in CultureDatas.Values)
        {
            foreach (var culture in Cultures)
                datas.TryAdd(culture, defaultValue);
        }
    }

    /// <summary>
    /// 为所有空值填充 <see cref="DefaultValue"/>
    /// </summary>
    public void FillDefaultValue() => FillDefaultValue(DefaultValue);
    #endregion

    /// <summary>
    /// 刷新所有I18nObject
    /// </summary>
    public void RefreshAllI18nObject()
    {
        foreach (var obj in I18nObjects)
        {
            obj.NotifyAllPropertyChanged();
        }
    }

    /// <summary>
    /// 复制数据到另一个资源
    /// </summary>
    /// <param name="otherResource">另一个资源</param>
    /// <param name="override">覆盖原始数据</param>
    public void CopyDataTo(I18nResource<TKey, TValue> otherResource, bool @override = false)
    {
        foreach (var culture in Cultures)
        {
            otherResource.AddCulture(culture);
        }
        foreach (var pair in CultureDatas)
        {
            foreach (var dataInfo in pair.Value)
            {
                if (@override)
                    otherResource.SetCultureData(dataInfo.Key, pair.Key, dataInfo.Value);
                else
                    otherResource.AddCultureData(dataInfo.Key, pair.Key, dataInfo.Value);
            }
        }
    }

    /// <summary>
    /// 复制指定数据到另一个资源
    /// </summary>
    /// <param name="otherResource"></param>
    /// <param name="key">键</param>
    /// <param name="override">覆盖原始数据</param>
    public void CopyDataTo(
        I18nResource<TKey, TValue> otherResource,
        TKey key,
        bool @override = false
    )
    {
        foreach (var culture in Cultures)
        {
            otherResource.AddCulture(culture);
        }
        if (CultureDatas.TryGetValue(key, out var pair) is false)
            return;
        foreach (var dataInfo in pair)
        {
            if (@override)
                otherResource.SetCultureData(dataInfo.Key, key, dataInfo.Value);
            else
                otherResource.AddCultureData(dataInfo.Key, key, dataInfo.Value);
        }
    }

    /// <summary>
    /// 复制指定数据到另一个资源
    /// </summary>
    /// <param name="otherResource"></param>
    /// <param name="keys">键</param>
    /// <param name="override">覆盖原始数据</param>
    public void CopyDataTo(
        I18nResource<TKey, TValue> otherResource,
        IEnumerable<TKey> keys,
        bool @override = false
    )
    {
        foreach (var culture in Cultures)
        {
            otherResource.AddCulture(culture);
        }
        foreach (var key in keys)
        {
            if (CultureDatas.TryGetValue(key, out var pair) is false)
                return;
            foreach (var dataInfo in pair)
            {
                if (@override)
                    otherResource.SetCultureData(dataInfo.Key, key, dataInfo.Value);
                else
                    otherResource.AddCultureData(dataInfo.Key, key, dataInfo.Value);
            }
        }
    }

    /// <summary>
    /// 清空所有文化和数据
    /// </summary>
    public void Clear()
    {
        I18nObjects.Clear();
        ClearCultureData();
        ClearCulture();
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
