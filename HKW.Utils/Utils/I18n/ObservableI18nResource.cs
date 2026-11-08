using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 可观测的I18n资源
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <remarks>
/// 资源及其已归属数据行不支持并发写入. 一次操作先捕获通知订阅, Changing 全部成功后提交完整矩阵,
/// 再发布 Changed. Changing 异常不提交; Changed 异常保留完整新态并传播, 不回滚外部副作用.
/// 通知期间允许读取和订阅变更, 拒绝同一资源及其数据行的重入写入. 自定义比较器必须稳定且不可重入.
/// 单个已有单元格更新不复制整行; 批量或结构写入准备受影响行的副本. 文化索引始终按数值连续排列.
/// 无当前文化时 GetData 抛 InvalidOperationException, TryGetData 返回 false, OrDefault 返回回退值.
/// Dispose 逐行解除通知转发归属, 不清空外部持有的旧行, 且不发布资源通知; 释放后资源只允许读取空态.
/// </remarks>
[DebuggerDisplay(
    "Name = {ResourceName}, KeyCount = {DatasByKey.Count}, CultureCount = {Cultures.Count}"
)]
public sealed class ObservableI18nResource<TKey, TValue>
    : II18nResource,
        INotifyPropertyChanged,
        II18nCultureParticipant
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="resourceName">资源名称</param>
    /// <param name="keyComparer">键比较器</param>
    /// <param name="valueComparer">值比较器</param>
    public ObservableI18nResource(
        string resourceName,
        EqualityComparer<TKey>? keyComparer = null,
        EqualityComparer<TValue>? valueComparer = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ResourceName = resourceName;

        KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        ValueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        _datas = new(KeyComparer);
        DatasByKey = new(_datas);
    }

    /// <inheritdoc/>
    /// <param name="resourceName">资源名称</param>
    /// <param name="currentCulture">当前文化</param>
    /// <param name="keyComparer">键比较器</param>
    /// <param name="valueComparer">值比较器</param>
    public ObservableI18nResource(
        string resourceName,
        CultureInfo? currentCulture,
        EqualityComparer<TKey>? keyComparer = null,
        EqualityComparer<TValue>? valueComparer = null
    )
        : this(resourceName, keyComparer, valueComparer)
    {
        if (currentCulture is not null)
        {
            _indexByCulture.Add(currentCulture, 0);
            _cultures.Add(currentCulture);
            _currentCulture = currentCulture;
        }
    }

    /// <inheritdoc/>
    /// <param name="resourceName">资源名称</param>
    /// <param name="cultures">文化</param>
    /// <param name="currentCulture">当前文化</param>
    /// <param name="keyComparer">键比较器</param>
    /// <param name="valueComparer">值比较器</param>
    public ObservableI18nResource(
        string resourceName,
        CultureInfo? currentCulture,
        IEnumerable<CultureInfo> cultures,
        EqualityComparer<TKey>? keyComparer = null,
        EqualityComparer<TValue>? valueComparer = null
    )
        : this(resourceName, currentCulture, keyComparer, valueComparer)
    {
        ArgumentNullException.ThrowIfNull(cultures);
        foreach (var culture in cultures)
        {
            ArgumentNullException.ThrowIfNull(culture);
            if (_indexByCulture.TryAdd(culture, _indexByCulture.Count))
                _cultures.Add(culture);
        }
    }

    private readonly ResourceDictionary _datas;
    private bool _writing;
    private CultureInfo? _currentCulture;

    internal Mutation BeginMutation() => new(this);

    II18nCultureChange II18nCultureParticipant.CaptureCultureChange(CultureInfo? culture) =>
        new CoreCultureChange(this, culture);

    // 复用资源操作快照和观察器冻结, 只拆分文化字段的提交与发布, 不改变矩阵算法.
    private sealed class CoreCultureChange : II18nCultureChange
    {
        private readonly Mutation _mutation;
        private readonly ObservableI18nResource<TKey, TValue> _source;
        private readonly bool _changed;

        internal CoreCultureChange(
            ObservableI18nResource<TKey, TValue> source,
            CultureInfo? culture
        )
        {
            _source = source;
            _mutation = source.BeginMutation();
            _changed = Equals(source._currentCulture, culture) is false;
            _mutation.CurrentCulture = culture;
        }

        public void Commit() => _source._currentCulture = _mutation.CurrentCulture;

        public void Publish() => _mutation.PublishCoreCulture(_changed);

        public void Dispose() => _mutation.Dispose();
    }

    internal void CheckWrite()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_writing)
            throw new InvalidOperationException(
                "Reentrant writes to the same resource are not allowed."
            );
    }

    /// <summary>
    /// 键比较器
    /// </summary>
    public EqualityComparer<TKey> KeyComparer { get; }

    /// <summary>
    /// 值比较器
    /// </summary>
    public EqualityComparer<TValue> ValueComparer { get; }

    /// <summary>
    /// 按键分类的数据, (key, List(cultureIndex, value))
    /// </summary>
    /// <remarks>
    /// 数据表结构只读. 使用 AddData, ReplaceData, RemoveData 或 ClearData 管理行, 已归属的行只允许修改单元格.
    /// </remarks>
    public ReadOnlyObservableDictionary<
        TKey,
        ObservableCultureDataList<TKey, TValue>
    > DatasByKey { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<CultureInfo, int> _indexByCulture = new();

    /// <summary>
    /// 文化和文化索引, (Culture, CultureIndex)
    /// </summary>
    public ReadOnlyDictionary<CultureInfo, int> IndexByCulture => field ??= new(_indexByCulture);

    /// <inheritdoc/>
    public ICollection<CultureInfo> Cultures => ObservableCultures;

    private readonly ObservableCultureDataList<int, CultureInfo> _cultures = new(0);

    /// <summary>
    /// 可观测的文化
    /// </summary>
    public ReadOnlyObservableList<CultureInfo> ObservableCultures => field ??= new(_cultures);

    /// <inheritdoc/>
    public string ResourceName { get; }

    /// <inheritdoc/>
    public CultureInfo? CurrentCulture
    {
        get => _currentCulture;
        set
        {
            using var operation = BeginMutation();
            if (value is not null && _indexByCulture.ContainsKey(value) is false)
                throw new ArgumentException(
                    "The culture is not registered in this resource.",
                    nameof(value)
                );
            operation.CurrentCulture = value;
            operation.Execute();
        }
    }

    /// <summary>
    /// 默认值, 指添加键时为各文化的值填充的占位符
    /// </summary>
    public TValue DefaultValue
    {
        get;
        set
        {
            CheckWrite();
            field = value;
        }
    } = default!;

    /// <summary>
    /// 获取默认值, 用于 <see cref="GetDataOrDefault(TKey, CultureInfo?)"/> 等参数有 <see cref="GetDefaultCultureDataHandler{TKey, TValue}"/> 的相关操作
    /// </summary>
    public GetDefaultCultureDataHandler<TKey, TValue> GetDefaultValue
    {
        get;
        set
        {
            CheckWrite();
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    } = (_, _) => default!;

    /// <summary>
    /// 记录观察者, 启用后以此为资源的 <see cref="II18nObserver{TKey,TValue}"/> 会被记录至 <see cref="Observers"/>
    /// </summary>
    /// <remarks>
    /// 仅记录启用期间新建或切换至此资源的观察者, 不回填历史观察者.
    /// 关闭时清除记录但不释放观察者; 再次启用仍从空记录开始.
    /// </remarks>
    [DefaultValue(false)]
    public bool RecordObserver
    {
        get;
        set
        {
            CheckWrite();
            field = value;
            if (value is false)
                _observers.Clear();
        }
    }

    private readonly List<II18nObserver<TKey, TValue>> _observers = [];

    /// <summary>
    /// 观察者, 只读且按实例唯一记录. 资源不拥有观察者的释放责任.
    /// </summary>
    public ReadOnlyCollection<II18nObserver<TKey, TValue>> Observers =>
        field ??= _observers.AsReadOnly();

    internal void RegisterObserver(II18nObserver<TKey, TValue> observer)
    {
        CheckWrite();
        if (RecordObserver && _observers.Contains(observer) is false)
            _observers.Add(observer);
    }

    internal void UnregisterObserver(II18nObserver<TKey, TValue> observer)
    {
        CheckObserverWrite();
        _observers.Remove(observer);
    }

    internal void CheckObserverWrite()
    {
        if (_writing)
            throw new InvalidOperationException(
                "Observer registration cannot change during a resource operation."
            );
    }

    // 观察者在整个资源操作开始时捕获目标快照, 与记录开关无关.
    internal event Func<IDisposable>? ObserverOperationStarting;

    /// <summary>
    /// 获取当前文化数据, 提供 this[]
    /// </summary>
    public GetDataCore GetCurrentCultureData => _getData ??= new(this);
    private GetDataCore? _getData;

    /// <summary>
    /// 获取当前文化数据或默认, 提供 this[]
    /// </summary>
    public GetDataOrDefaultCore GetCurrentCultureDataOrDefault => _getDataOrDefault ??= new(this);
    private GetDataOrDefaultCore? _getDataOrDefault;

    /// <summary>
    /// 包含数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否包含</returns>
    public bool ContainsData(TKey key)
    {
        return DatasByKey.ContainsKey(key);
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>值</returns>
    /// <exception cref="KeyNotFoundException">当键不存在时</exception>
    public TValue GetData(TKey key, CultureInfo? culture = null)
    {
        culture ??= CurrentCulture;
        if (culture is null)
            throw new InvalidOperationException("There is no current culture.");
        if (_indexByCulture.TryGetValue(culture, out var index) is false)
            throw new KeyNotFoundException("The culture is not registered in this resource.");
        return DatasByKey[key][index];
    }

    /// <summary>
    /// 尝试获取数据. 缺少键, 文化或当前文化时返回 false, 默认值本身仍是合法数据.
    /// </summary>
    public bool TryGetData(
        TKey key,
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value,
        CultureInfo? culture = null
    )
    {
        ArgumentNullException.ThrowIfNull(key);
        culture ??= CurrentCulture;
        if (
            culture is not null
            && _indexByCulture.TryGetValue(culture, out var index)
            && DatasByKey.TryGetValue(key, out var row)
        )
        {
            value = row[index];
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// 获取数据或从 <see cref="GetDefaultValue"/> 获取的默认值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>值</returns>
    public TValue GetDataOrDefault(TKey key, CultureInfo? culture = null)
    {
        culture ??= CurrentCulture;
        if (DatasByKey.TryGetValue(key, out var data) is false)
            return GetDefaultValue(key, culture);
        if (culture is null || _indexByCulture.TryGetValue(culture, out var index) is false)
            return GetDefaultValue(key, culture);
        return data[index];
    }

    /// <summary>
    /// 获取数据或默认
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    public TValue GetDataOrDefault(TKey key, TValue defaultValue, CultureInfo? culture = null)
    {
        culture ??= CurrentCulture;
        if (DatasByKey.TryGetValue(key, out var data) is false)
            return defaultValue;
        if (culture is null || _indexByCulture.TryGetValue(culture, out var index) is false)
            return defaultValue;
        return data[index];
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool SetData(TKey key, TValue value, CultureInfo? culture = null)
    {
        using var operation = BeginMutation();
        var result = operation.SetSingleData(key, value, culture ?? CurrentCulture, false);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="pairs">数据对</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> 为 <see langword="null"/></exception>
    public void SetDatas(IEnumerable<KeyValuePair<TKey, TValue>> pairs, CultureInfo? culture = null)
    {
        using var operation = BeginMutation();
        ArgumentNullException.ThrowIfNull(pairs);
        culture ??= CurrentCulture;
        operation.CaptureRows();
        foreach (var pair in pairs.ToArray())
            operation.SetData(pair.Key, pair.Value, culture, false);
        operation.Execute();
    }

    /// <summary>
    /// 为为 <see cref="DefaultValue"/> 的值设置数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool SetDataWhenDefault(TKey key, TValue value, CultureInfo? culture = null)
    {
        using var operation = BeginMutation();
        var result = operation.SetSingleData(key, value, culture ?? CurrentCulture, true);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 为为 <see cref="DefaultValue"/> 的值设置数据
    /// </summary>
    /// <param name="pairs">键值对</param>
    /// <param name="culture">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> 为 <see langword="null"/></exception>
    public void SetDatasWhenDefault(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs,
        CultureInfo? culture = null
    )
    {
        using var operation = BeginMutation();
        ArgumentNullException.ThrowIfNull(pairs);
        culture ??= CurrentCulture;
        operation.CaptureRows();
        foreach (var pair in pairs.ToArray())
            operation.SetData(pair.Key, pair.Value, culture, true);
        operation.Execute();
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否删除成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool RemoveData(TKey key)
    {
        using var operation = BeginMutation();
        var result = operation.RemoveData(key);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public void ClearData()
    {
        using var operation = BeginMutation();
        operation.ClearData();
        operation.Execute();
    }

    /// <summary>
    /// 添加数据行. 行必须未归属且键匹配, 缺少的单元格使用默认值补齐.
    /// </summary>
    public bool AddData(TKey key, ObservableCultureDataList<TKey, TValue> row)
    {
        using var operation = BeginMutation();
        var result = operation.PutRow(key, row, false);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 替换已存在的数据行. 新行必须未归属且键匹配, 旧行保留数据并与资源脱钩.
    /// </summary>
    public bool ReplaceData(TKey key, ObservableCultureDataList<TKey, TValue> row)
    {
        using var operation = BeginMutation();
        if (DatasByKey.ContainsKey(key) is false)
            return false;
        var result = operation.PutRow(key, row, true);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 当 <see cref="RecordObserver"/> 启用时, 删除所有未被观察的数据
    /// </summary>
    public void ClearUnobservedData()
    {
        using var operation = BeginMutation();
        if (RecordObserver is false)
            return;
        var observedKeys = _observers
            .SelectMany(static x => x.Registrations)
            .Select(static x => x.OldKey)
            .ToHashSet(KeyComparer);

        if (observedKeys.Count == 0)
            operation.ClearData();
        else
        {
            foreach (
                var key in DatasByKey
                    .Keys.Where(key => observedKeys.Contains(key) is false)
                    .ToArray()
            )
                operation.RemoveData(key);
        }
        operation.Execute();
    }

    /// <summary>
    /// 复制数据
    /// </summary>
    /// <param name="oldKey">旧键</param>
    /// <param name="newKey">新键</param>
    /// <param name="overwrite">覆盖</param>
    /// <returns>是否成功</returns>
    public bool CopyData(TKey oldKey, TKey newKey, bool overwrite)
    {
        using var operation = BeginMutation();
        ArgumentNullException.ThrowIfNull(oldKey);
        ArgumentNullException.ThrowIfNull(newKey);
        if (KeyComparer.Equals(oldKey, newKey))
            return false;
        if (DatasByKey.TryGetValue(oldKey, out var data) is false)
            return false;
        var newData = new ObservableCultureDataList<TKey, TValue>(newKey, data);
        var result = operation.PutRow(newKey, newData, overwrite);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 复制数据至
    /// </summary>
    /// <param name="resource">资源</param>
    /// <param name="keys">指定键</param>
    public void CopyDataTo(ObservableI18nResource<TKey, TValue> resource, params TKey[] keys)
    {
        ArgumentNullException.ThrowIfNull(resource);
        using var sourceOperation = BeginMutation();
        if (ReferenceEquals(this, resource))
            return;
        using var operation = resource.BeginMutation();
        var selected = keys is null || keys.Length == 0 ? null : keys.ToHashSet(KeyComparer);
        foreach (var culture in _cultures)
            operation.AddCulture(culture);
        foreach (var pair in DatasByKey)
        {
            if (selected is not null && selected.Contains(pair.Key) is false)
                continue;
            for (var i = 0; i < _cultures.Count; i++)
                operation.SetData(pair.Key, pair.Value[i], _cultures[i], false);
        }
        operation.Execute();
    }

    /// <summary>
    /// 包含文化
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>是否包含</returns>
    public bool HasCulture(CultureInfo culture)
    {
        return _indexByCulture.ContainsKey(culture);
    }

    /// <summary>
    /// 从文化获取索引
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>文化索引</returns>
    public int GetCultureIndex(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        if (_indexByCulture.TryGetValue(culture, out var index))
            return index;
        return -1;
    }

    /// <summary>
    /// 从文化获取索引
    /// </summary>
    /// <param name="cultureName">文化</param>
    /// <returns>文化索引</returns>
    public int GetCultureIndex(string cultureName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(cultureName);
        for (var i = 0; i < _cultures.Count; i++)
        {
            if (_cultures[i].Name == cultureName)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 从名称获取文化
    /// </summary>
    /// <param name="cultureName">名称</param>
    /// <returns>文化</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="cultureName"/> 数据错误, 名称未注册</exception>
    public CultureInfo GetCulture(string cultureName)
    {
        var index = GetCultureIndex(cultureName);
        if (index < 0)
            throw new KeyNotFoundException("The culture is not registered in this resource.");
        return _cultures[index];
    }

    /// <summary>
    /// 尝试从名称获取文化
    /// </summary>
    /// <param name="cultureName">名称</param>
    /// <param name="culture">文化</param>
    /// <returns>是否成功</returns>
    public bool TryGetCulture(string cultureName, [MaybeNullWhen(false)] out CultureInfo culture)
    {
        culture = null!;
        if (string.IsNullOrWhiteSpace(cultureName))
            return false;
        var index = GetCultureIndex(cultureName);
        if (index < 0)
            return false;
        culture = _cultures[index];
        return true;
    }

    /// <summary>
    /// 从索引获取文化
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>文化</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 范围错误</exception>
    public CultureInfo GetCulture(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _cultures.Count);
        return _cultures[index];
    }

    /// <summary>
    /// 尝试从索引获取文化
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="culture">文化</param>
    /// <returns>是否成功</returns>
    public bool TryGetCulture(int index, [MaybeNullWhen(false)] out CultureInfo culture)
    {
        culture = null!;
        if (index < 0 || index >= _cultures.Count)
            return false;
        culture = _cultures[index];
        return true;
    }

    /// <summary>
    /// 添加文化, 会自动为所有键的新文化添加 <see cref="DefaultValue"/>
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>是否添加成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="culture"/> 为 <see langword="null"/></exception>
    public bool AddCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(culture.Name);
        using var operation = BeginMutation();
        var result = operation.AddCulture(culture);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 删除文化. 原约束 "禁止删除 <see cref="CurrentCulture"/>" 已取消, 删除当前文化后进入无当前文化状态.
    /// </summary>
    /// <param name="culture">文化</param>
    /// <returns>是否删除成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="culture"/> 为 <see langword="null"/></exception>
    public bool RemoveCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        using var operation = BeginMutation();
        var result = operation.RemoveCulture(culture);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 替换文化
    /// </summary>
    /// <param name="oldCultureInfo">旧文化</param>
    /// <param name="culture">文化</param>
    /// <returns>是否添加成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="culture"/> 为 <see langword="null"/></exception>
    public bool ReplaceCulture(CultureInfo oldCultureInfo, CultureInfo culture)
    {
        using var operation = BeginMutation();
        var result = operation.ReplaceCulture(oldCultureInfo, culture);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 清除所有文化
    /// </summary>
    public void ClearCulture()
    {
        using var operation = BeginMutation();
        operation.ClearCulture();
        operation.Execute();
    }

    /// <summary>
    /// 设置主要文化
    /// </summary>
    public void SetCurrentCulture(CultureInfo? culture)
    {
        CurrentCulture = culture;
    }

    /// <summary>
    /// 重命名键
    /// </summary>
    /// <param name="oldKey">旧键</param>
    /// <param name="newKey">新键</param>
    /// <param name="overwrite">若 <paramref name="newKey"/> 存在则覆盖</param>
    /// <returns>是否重命名成功</returns>
    public bool RenameKey(TKey oldKey, TKey newKey, bool overwrite = false)
    {
        using var operation = BeginMutation();
        var result = operation.RenameKey(oldKey, newKey, overwrite);
        operation.Execute();
        return result;
    }

    /// <summary>
    /// 先准备全部行和订阅快照, 再统一提交, 最后发布通知. 内部组合操作不重新进入写保护.
    /// </summary>
    internal sealed class Mutation : IDisposable
    {
        private readonly ObservableI18nResource<TKey, TValue> _source;
        private readonly List<IDisposable> _leases = [];
        private readonly HashSet<ObservableCultureDataList<TKey, TValue>> _captured = [];
        private readonly Dictionary<
            ObservableCultureDataList<TKey, TValue>,
            List<TValue>
        > _values = [];
        private readonly List<(
            ObservableCultureDataList<TKey, TValue> Row,
            NotifyListChangeEventArgs<TValue> Change
        )> _rowChanges = [];
        private readonly List<(
            NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataList<TKey, TValue>> Change,
            int Index
        )> _dictionaryChanges = [];
        private readonly List<NotifyListChangeEventArgs<CultureInfo>> _cultureChanges = [];
        private readonly Dictionary<ObservableCultureDataList<TKey, TValue>, TKey> _keys = [];
        private readonly List<CultureDataChangedEventArgs<TKey, TValue>> _invalidations = [];
        private readonly List<ObservableCultureDataList<TKey, TValue>> _reserved = [];
        private readonly EventHandler<CultureInfo?>? _currentChanged;
        private readonly PropertyChangedEventHandler? _propertyChanged;
        private readonly CultureDataChangedEventHandler<TKey, TValue>? _dataChanged;
        private readonly KeyRenamedEventHandler<TKey, TValue>? _keyRenamed;
        private readonly Action? _refreshData;
        private readonly Action? _refreshDefault;
        private Dictionary<TKey, ObservableCultureDataList<TKey, TValue>>? _rows;
        private List<CultureInfo>? _cultures;
        private (TKey OldKey, TKey NewKey)? _renamed;
        private bool _refresh;
        private bool _disposed;

        internal Mutation(ObservableI18nResource<TKey, TValue> source)
        {
            ObjectDisposedException.ThrowIf(source._disposed, source);
            if (source._writing)
                throw new InvalidOperationException(
                    "Reentrant writes to the same resource are not allowed."
                );
            _source = source;
            source._writing = true;
            CurrentCulture = source.CurrentCulture;
            _currentChanged = source.CurrentCultureChanged;
            _propertyChanged = source.PropertyChanged;
            _dataChanged = source.CultureDataChanged;
            _keyRenamed = source.KeyRenamed;
            _refreshData = source._getData?.Capture();
            _refreshDefault = source._getDataOrDefault?.Capture();
            try
            {
                _leases.Add(source._datas.Capture());
                _leases.Add(source._cultures.Capture());
                if (source.ObserverOperationStarting is { } observers)
                {
                    foreach (var capture in observers.GetInvocationList().Cast<Func<IDisposable>>())
                        _leases.Add(capture());
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        internal CultureInfo? CurrentCulture { get; set; }
        private IReadOnlyDictionary<TKey, ObservableCultureDataList<TKey, TValue>> Rows =>
            _rows is null ? _source._datas : _rows;
        private Dictionary<TKey, ObservableCultureDataList<TKey, TValue>> WritableRows =>
            _rows ??= new(_source._datas, _source.KeyComparer);
        private IReadOnlyList<CultureInfo> Cultures =>
            _cultures is null ? _source._cultures : _cultures;
        private List<CultureInfo> WritableCultures => _cultures ??= new(_source._cultures);

        private void CaptureRow(ObservableCultureDataList<TKey, TValue> row)
        {
            if (_captured.Add(row))
                _leases.Add(row.Capture());
        }

        private List<TValue> Values(ObservableCultureDataList<TKey, TValue> row)
        {
            if (_values.TryGetValue(row, out var values))
                return values;
            CaptureRow(row);
            values = new(row);
            _values.Add(row, values);
            return values;
        }

        private int CultureIndex(CultureInfo? culture)
        {
            if (culture is null)
                return -1;
            if (_cultures is null)
                return _source.GetCultureIndex(culture);
            return _cultures.IndexOf(culture);
        }

        private void Invalidate(TKey key)
        {
            _invalidations.Add(new(key, default, default, null));
            _refresh = true;
        }

        private (ObservableCultureDataList<TKey, TValue> Row, int Index, TValue Value)? _cellCommit;

        internal bool SetSingleData(TKey key, TValue value, CultureInfo? culture, bool whenDefault)
        {
            ArgumentNullException.ThrowIfNull(key);
            var index = CultureIndex(culture);
            if (index < 0)
                return false;
            if (Rows.TryGetValue(key, out var row) is false)
                return SetData(key, value, culture, whenDefault);
            var old = row[index];
            if (whenDefault && _source.ValueComparer.Equals(old, _source.DefaultValue) is false)
                return false;
            if (EqualityComparer<TValue>.Default.Equals(old, value))
                return true;
            CaptureRow(row);
            _cellCommit = (row, index, value);
            _rowChanges.Add((row, new(ListChangeAction.Replace, value, old, index)));
            _invalidations.Add(new(row.Key, old, value, culture));
            _refresh = Equals(culture, CurrentCulture);
            return true;
        }

        internal void CaptureRows()
        {
            foreach (var row in Rows.Values)
                CaptureRow(row);
        }

        internal bool SetData(TKey key, TValue value, CultureInfo? culture, bool whenDefault)
        {
            ArgumentNullException.ThrowIfNull(key);
            var index = CultureIndex(culture);
            if (index < 0)
                return false;
            var created = Rows.TryGetValue(key, out var row) is false;
            if (created)
            {
                row = new(key, Enumerable.Repeat(_source.DefaultValue, Cultures.Count));
                PutRow(key, row, false, false);
            }
            var values = Values(row!);
            var old = values[index];
            if (whenDefault && _source.ValueComparer.Equals(old, _source.DefaultValue) is false)
                return false;
            if (EqualityComparer<TValue>.Default.Equals(old, value))
            {
                if (created)
                    _invalidations.Add(new(row!.Key, old, value, culture));
                return true;
            }
            values[index] = value;
            _rowChanges.Add((row!, new(ListChangeAction.Replace, value, old, index)));
            _invalidations.Add(new(row!.Key, old, value, culture));
            _refresh |= Equals(culture, CurrentCulture);
            return true;
        }

        internal bool PutRow(
            TKey key,
            ObservableCultureDataList<TKey, TValue> row,
            bool overwrite,
            bool invalidate = true
        )
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(row);
            if (_source.KeyComparer.Equals(key, row.Key) is false)
                throw new ArgumentException(
                    "The row key must match the dictionary key.",
                    nameof(row)
                );
            if (row.Owner is not null || row.Reservation is not null)
                throw new InvalidOperationException(
                    "A row cannot belong to multiple keys or resources."
                );
            if (row.Count > Cultures.Count)
                throw new ArgumentException(
                    "The culture data count cannot exceed the culture count.",
                    nameof(row)
                );
            var exists = Rows.TryGetValue(key, out var previous);
            if (exists && overwrite is false)
                return false;
            CaptureRow(row);
            row.Reservation = _source;
            _reserved.Add(row);
            if (previous is not null)
            {
                CaptureRow(previous);
                key = previous.Key;
            }
            if (EqualityComparer<TKey>.Default.Equals(row.Key, key) is false)
                _keys[row] = key;
            var values = Values(row);
            for (var i = values.Count; i < Cultures.Count; i++)
            {
                values.Add(_source.DefaultValue);
                _rowChanges.Add((row, new(ListChangeAction.Add, _source.DefaultValue, i)));
            }
            WritableRows[key] = row;
            var pair = KeyValuePair.Create(key, row);
            var change = exists
                ? new NotifyDictionaryChangeEventArgs<
                    TKey,
                    ObservableCultureDataList<TKey, TValue>
                >(DictionaryChangeAction.Replace, pair, new(key, previous!))
                : new NotifyDictionaryChangeEventArgs<
                    TKey,
                    ObservableCultureDataList<TKey, TValue>
                >(DictionaryChangeAction.Add, pair);
            _dictionaryChanges.Add((change, -1));
            if (invalidate)
                Invalidate(key);
            else
                _refresh = true;
            return true;
        }

        internal bool RemoveData(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            if (Rows.TryGetValue(key, out var row) is false)
                return false;
            CaptureRow(row);
            var index = Rows.Keys.ToList().FindIndex(k => _source.KeyComparer.Equals(k, key));
            WritableRows.Remove(key);
            _dictionaryChanges.Add((new(DictionaryChangeAction.Remove, new(row.Key, row)), index));
            Invalidate(row.Key);
            return true;
        }

        internal void ClearData()
        {
            if (Rows.Count == 0)
                return;
            foreach (var pair in Rows)
            {
                CaptureRow(pair.Value);
                Invalidate(pair.Key);
            }
            WritableRows.Clear();
            _dictionaryChanges.Add(
                (
                    NotifyDictionaryChangeEventArgs<
                        TKey,
                        ObservableCultureDataList<TKey, TValue>
                    >.Cache_Clear,
                    -1
                )
            );
        }

        internal bool RenameKey(TKey oldKey, TKey newKey, bool overwrite)
        {
            ArgumentNullException.ThrowIfNull(oldKey);
            ArgumentNullException.ThrowIfNull(newKey);
            if (
                _source.KeyComparer.Equals(oldKey, newKey)
                || Rows.TryGetValue(oldKey, out var row) is false
            )
                return false;
            if (Rows.TryGetValue(newKey, out var target))
            {
                if (overwrite is false)
                    return false;
                CaptureRow(target);
                newKey = target.Key;
            }
            CaptureRow(row);
            oldKey = row.Key;
            var oldIndex = Rows
                .Keys.ToList()
                .FindIndex(key => _source.KeyComparer.Equals(key, oldKey));
            WritableRows.Remove(oldKey);
            WritableRows[newKey] = row;
            _keys[row] = newKey;
            _dictionaryChanges.Add(
                (new(DictionaryChangeAction.Remove, new(oldKey, row)), oldIndex)
            );
            var newPair = KeyValuePair.Create(newKey, row);
            var change = target is null
                ? new NotifyDictionaryChangeEventArgs<
                    TKey,
                    ObservableCultureDataList<TKey, TValue>
                >(DictionaryChangeAction.Add, newPair)
                : new NotifyDictionaryChangeEventArgs<
                    TKey,
                    ObservableCultureDataList<TKey, TValue>
                >(DictionaryChangeAction.Replace, newPair, new(newKey, target));
            _dictionaryChanges.Add((change, -1));
            _renamed = (oldKey, newKey);
            Invalidate(oldKey);
            Invalidate(newKey);
            return true;
        }

        internal bool AddCulture(CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(culture);
            if (CultureIndex(culture) >= 0)
                return false;
            var index = Cultures.Count;
            WritableCultures.Add(culture);
            _cultureChanges.Add(new(ListChangeAction.Add, culture, index));
            foreach (var row in Rows.Values)
            {
                Values(row).Add(_source.DefaultValue);
                _rowChanges.Add((row, new(ListChangeAction.Add, _source.DefaultValue, index)));
                _invalidations.Add(new(row.Key, default, _source.DefaultValue, culture));
            }
            return true;
        }

        internal bool RemoveCulture(CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(culture);
            var index = CultureIndex(culture);
            if (index < 0)
                return false;
            WritableCultures.RemoveAt(index);
            _cultureChanges.Add(new(ListChangeAction.Remove, culture, index));
            foreach (var row in Rows.Values)
            {
                var values = Values(row);
                var old = values[index];
                values.RemoveAt(index);
                _rowChanges.Add((row, new(ListChangeAction.Remove, old, index)));
                _invalidations.Add(new(row.Key, old, default, culture));
            }
            if (Equals(CurrentCulture, culture))
                CurrentCulture = null;
            return true;
        }

        internal bool ReplaceCulture(CultureInfo oldCulture, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(oldCulture);
            ArgumentNullException.ThrowIfNull(culture);
            var index = CultureIndex(oldCulture);
            if (index < 0 || CultureIndex(culture) >= 0)
                return false;
            WritableCultures[index] = culture;
            _cultureChanges.Add(new(ListChangeAction.Replace, culture, oldCulture, index));
            foreach (var row in Rows.Values)
                Invalidate(row.Key);
            if (Equals(CurrentCulture, oldCulture))
                CurrentCulture = culture;
            return true;
        }

        internal void ClearCulture()
        {
            if (Cultures.Count == 0)
                return;
            WritableCultures.Clear();
            _cultureChanges.Add(NotifyListChangeEventArgs<CultureInfo>.Cache_Clear);
            foreach (var row in Rows.Values)
            {
                Values(row).Clear();
                _rowChanges.Add((row, NotifyListChangeEventArgs<TValue>.Cache_Clear));
                Invalidate(row.Key);
            }
            CurrentCulture = null;
        }

        internal void Execute()
        {
            foreach (var (change, _) in _dictionaryChanges)
                _source._datas.Prepare(change);
            foreach (var (row, change) in _rowChanges)
                row.Prepare(change);
            foreach (var change in _cultureChanges)
                _source._cultures.Prepare(change);

            if (_cellCommit is { } cell)
                cell.Row.CommitCell(cell.Index, cell.Value);
            foreach (var (row, values) in _values)
                row.Commit(values);
            foreach (var (row, key) in _keys)
                row.SetKeyCore(key);
            if (_rows is not null)
            {
                foreach (var row in _source._datas.Values)
                    row.Owner = null;
                _source._datas.Commit(_rows);
                foreach (var row in _rows.Values)
                    row.Owner = _source;
            }
            if (_cultures is not null)
            {
                _source._cultures.Commit(_cultures);
                _source._indexByCulture.Clear();
                for (var i = 0; i < _cultures.Count; i++)
                    _source._indexByCulture.Add(_cultures[i], i);
            }
            var currentChanged = Equals(_source._currentCulture, CurrentCulture) is false;
            _source._currentCulture = CurrentCulture;

            foreach (var (row, change) in _rowChanges)
                row.Publish(change);
            foreach (var row in _keys.Keys)
                row.PublishKey();
            foreach (var (change, index) in _dictionaryChanges)
                _source._datas.Publish(change, index);
            foreach (var change in _cultureChanges)
                _source._cultures.Publish(change);
            if (_renamed is { } renamed)
                _keyRenamed?.Invoke(_source, renamed);
            if (currentChanged)
            {
                _currentChanged?.Invoke(_source, CurrentCulture);
                _propertyChanged?.Invoke(_source, PropertyChangedArgsCache.CurrentCulture);
            }
            foreach (var change in _invalidations)
                _dataChanged?.Invoke(_source, change);
            if (_refresh || currentChanged)
                Refresh();
        }

        internal void PublishCoreCulture(bool changed)
        {
            if (changed is false)
                return;
            _currentChanged?.Invoke(_source, CurrentCulture);
            _propertyChanged?.Invoke(_source, PropertyChangedArgsCache.CurrentCulture);
            Refresh();
        }

        internal void PublishCell(TKey key, TValue oldValue, TValue newValue, int index)
        {
            var culture = _source.GetCulture(index);
            _dataChanged?.Invoke(_source, new(key, oldValue, newValue, culture));
            if (Equals(culture, CurrentCulture))
                Refresh();
        }

        private void Refresh()
        {
            _refreshData?.Invoke();
            _refreshDefault?.Invoke();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            foreach (var row in _reserved)
                row.Reservation = null;
            for (var i = _leases.Count - 1; i >= 0; i--)
                _leases[i].Dispose();
            _source._writing = false;
        }
    }

    private sealed class ResourceDictionary(EqualityComparer<TKey> comparer)
        : ObservableDictionaryWrapper<
            TKey,
            ObservableCultureDataList<TKey, TValue>,
            Dictionary<TKey, ObservableCultureDataList<TKey, TValue>>
        >(new(comparer), comparer)
    {
        internal IDisposable Capture() => BeginOperation();

        internal void Prepare(
            NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataList<TKey, TValue>> change
        ) => OnDictionaryChanging(change);

        internal void Commit(Dictionary<TKey, ObservableCultureDataList<TKey, TValue>> rows)
        {
            SourceDictionary.Clear();
            foreach (var pair in rows)
                SourceDictionary.Add(pair.Key, pair.Value);
        }

        internal void Publish(
            NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataList<TKey, TValue>> change,
            int index
        )
        {
            change.TryGetNewPair(out var next);
            change.TryGetOldPair(out var old);
            switch (change.Action)
            {
                case DictionaryChangeAction.Add:
                    OnDictionaryAdded(change, next);
                    break;
                case DictionaryChangeAction.Remove:
                    OnDictionaryRemoved(change, old, index);
                    break;
                case DictionaryChangeAction.Replace:
                    OnDictionaryReplaced(change, next, old);
                    break;
                case DictionaryChangeAction.Clear:
                    OnDictionaryCleared();
                    break;
            }
        }
    }

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableI18nResource() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            if (_writing)
                throw new InvalidOperationException(
                    "Reentrant writes to the same resource are not allowed."
                );
            foreach (var row in _datas.Values)
                row.Owner = null;
            DatasByKey.Dispose();
            _datas.Commit(new(KeyComparer));
            _indexByCulture.Clear();
            _cultures.Commit([]);
            _currentCulture = null;
            _observers.Clear();
            ObserverOperationStarting = null;
            CurrentCultureChanged = null;
            CultureDataChanged = null;
            KeyRenamed = null;
            PropertyChanged = null;
        }
        _disposed = true;
    }
    #endregion

    /// <summary>
    /// 文化改变后事件
    /// </summary>
    public event EventHandler<CultureInfo?>? CurrentCultureChanged;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 文化数据改变事件
    /// </summary>
    /// <remarks>
    /// 单元格变更包含准确的新旧值和文化. 行新增, 删除, 替换, 重命名或清空以 CultureInfo 为 null
    /// 表示该键的全部文化数据失效, 此时默认值载荷不表示存在性. 订阅者应重新查询资源.
    /// 文化列增删包含对应文化; 替换或清空文化列使受影响键的全部文化失效.
    /// </remarks>
    public event CultureDataChangedEventHandler<TKey, TValue>? CultureDataChanged;

    /// <summary>
    /// 键重命名后事件
    /// </summary>
    public event KeyRenamedEventHandler<TKey, TValue>? KeyRenamed;

    /// <summary>
    /// 获取数据核心
    /// </summary>
    /// <param name="source">源</param>
    public sealed class GetDataCore(ObservableI18nResource<TKey, TValue> source)
        : INotifyPropertyChanged
    {
        private readonly ObservableI18nResource<TKey, TValue> _source = source;

        /// <summary>
        /// 使用 this[] 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        /// <exception cref="KeyNotFoundException">未找到键</exception>
        public TValue this[TKey key] => _source.GetData(key);

        internal Action Capture()
        {
            var handlers = PropertyChanged;
            return () => handlers?.InvokeIndexer(this);
        }

        /// <summary>
        /// 刷新 this[]
        /// </summary>
        public void Refresh()
        {
            PropertyChanged?.InvokeIndexer(this);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// 获取数据或默认核心
    /// </summary>
    /// <param name="source"></param>
    public sealed class GetDataOrDefaultCore(ObservableI18nResource<TKey, TValue> source)
        : INotifyPropertyChanged
    {
        private readonly ObservableI18nResource<TKey, TValue> _source = source;

        /// <summary>
        /// 使用 this[] 获取数据或默认
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public TValue this[TKey key] => _source.GetDataOrDefault(key, null);

        internal Action Capture()
        {
            var handlers = PropertyChanged;
            return () => handlers?.InvokeIndexer(this);
        }

        /// <summary>
        /// 刷新 this[]
        /// </summary>
        public void Refresh()
        {
            PropertyChanged?.InvokeIndexer(this);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

/// <summary>
/// 值改变行动处理器
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <param name="source">源</param>
/// <param name="key">键</param>
public delegate void ValueChangedActionHandler<in TKey>(INotifyPropertyChanged source, TKey key)
    where TKey : notnull;

/// <summary>
/// 获取默认文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="key">键</param>
/// <param name="culture">文化信息</param>
/// <returns>值</returns>
public delegate TValue GetDefaultCultureDataHandler<in TKey, out TValue>(
    TKey key,
    CultureInfo? culture
)
    where TKey : notnull;

/// <summary>
/// 文化数据改变后事件处理器
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void CultureDataChangedEventHandler<TKey, TValue>(
    II18nResource sender,
    CultureDataChangedEventArgs<TKey, TValue> e
)
    where TKey : notnull;

/// <summary>
/// 键重命名事件处理器
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void KeyRenamedEventHandler<TKey, TValue>(
    II18nResource sender,
    (TKey OldKey, TKey NewKey) e
)
    where TKey : notnull;
