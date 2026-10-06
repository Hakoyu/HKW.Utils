using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils;

/// <summary>
/// 可观测的I18n资源
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay(
    "Name = {ResourceName}, KeyCount = {DatasByKey.Count}, CultureCount = {Cultures.Count}"
)]
public sealed class ObservableI18nResource<TKey, TValue> : II18nResource, INotifyPropertyChanged
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
        DatasByKey = new(KeyComparer);
        DatasByKey.DictionaryChanging += DatasByKey_DictionaryChanging;
        DatasByKey.DictionaryChanged += DatasByKey_DictionaryChanged;

        CurrentCulture = null!;
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
        currentCulture ??= CultureInfo.CurrentCulture;
        _indexByCulture.Add(currentCulture, 0);
        CurrentCulture = currentCulture;
    }

    /// <inheritdoc/>
    /// <param name="resourceName">资源名称</param>
    /// <param name="cultures">文化</param>
    /// <param name="currentCulture">当前文化</param>
    /// <param name="keyComparer">键比较器</param>
    /// <param name="valueComparer">值比较器</param>
    public ObservableI18nResource(
        string resourceName,
        CultureInfo currentCulture,
        IEnumerable<CultureInfo> cultures,
        EqualityComparer<TKey>? keyComparer = null,
        EqualityComparer<TValue>? valueComparer = null
    )
        : this(resourceName, currentCulture, keyComparer, valueComparer)
    {
        foreach (var culture in cultures)
            _indexByCulture.TryAdd(culture, _indexByCulture.Count);
    }

    private void DatasByKey_DictionaryChanging(
        IObservableDictionary<TKey, ObservableCultureDataList<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataList<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Clear)
        {
            foreach (var value in sender.Values)
            {
                value.ListChanged -= Datas_ListChanged;
                value.Clear();
            }
        }
    }

#pragma warning disable S3776
    private void DatasByKey_DictionaryChanged(
#pragma warning restore S3776
        IObservableDictionary<TKey, ObservableCultureDataList<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataList<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair))
            {
                if (newPair.Value.Count == _indexByCulture.Count)
                    return;

                foreach (var culture in _indexByCulture)
                    newPair.Value.Add(DefaultValue);
                newPair.Value.ListChanged += Datas_ListChanged;
            }
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.ListChanged -= Datas_ListChanged;
            }
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.ListChanged -= Datas_ListChanged;
            }
            if (e.TryGetNewPair(out var newPair))
            {
                if (newPair.Value.Count == _indexByCulture.Count)
                    return;
                foreach (var culture in _indexByCulture)
                    newPair.Value.Add(DefaultValue);
                newPair.Value.ListChanged += Datas_ListChanged;
            }
        }
    }

    private void Datas_ListChanged(
        IObservableList<TValue> sender,
        NotifyListChangeEventArgs<TValue> e
    )
    {
        if (sender is not ObservableCultureDataList<TKey, TValue> dic)
            return;
        CultureDataChanged?.Invoke(
            this,
            new(dic.Key, e.OldItem, e.NewItem, _indexByCulture.GetAt(e.Index).Key)
        );

        Observable.DoActionsBy(dic.Key);
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
    public ObservableDictionary<TKey, ObservableCultureDataList<TKey, TValue>> DatasByKey { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly OrderedDictionary<CultureInfo, int> _indexByCulture = new();

    /// <summary>
    /// 文化和文化索引, (culture, cultureIndex)
    /// </summary>
    public ReadOnlyDictionary<CultureInfo, int> IndexByCulture => field ??= new(_indexByCulture);

    /// <inheritdoc/>
    public ICollection<CultureInfo> Cultures => _indexByCulture.Keys;

    /// <inheritdoc/>
    public string ResourceName { get; }

    /// <inheritdoc/>
    public CultureInfo CurrentCulture
    {
        get => field;
        set
        {
            if (field == value)
                return;
            ArgumentException.ThrowIfNotContains(_indexByCulture.Keys, value);
            field = value;
            CurrentCultureChanged?.Invoke(this, value);
            PropertyChanged?.Invoke(this, new(nameof(CurrentCulture)));
            GetCurrentCultureData?.Refresh();
            GetCurrentCultureDataOrDefault?.Refresh();

            foreach (var key in DatasByKey.Keys)
                Observable.DoActionsBy(key);
        }
    }

    /// <summary>
    /// 默认值, 指添加键时为各文化的值填充的占位符
    /// </summary>
    public TValue DefaultValue { get; set; } = default!;

    /// <summary>
    /// 获取默认值, 用于 <see cref="GetDataOrDefault(TKey, CultureInfo?)"/> 等参数有 <see cref="GetDefaultCultureDataHandler{TKey, TValue}"/> 的相关操作
    /// </summary>
    public GetDefaultCultureDataHandler<TKey, TValue> GetDefaultValue { get; set; } =
        (_, _) => default!;

    /// <summary>
    /// 获取当前文化数据, 提供 this[]
    /// </summary>
    public GetDataCore GetCurrentCultureData => field ??= new(this);

    /// <summary>
    /// 获取当前文化数据或默认, 提供 this[]
    /// </summary>
    public GetDataOrDefaultCore GetCurrentCultureDataOrDefault => field ??= new(this);

    /// <summary>
    /// 可观察核心, 提供与 <see cref="INotifyPropertyChanged"/> 的联动接口
    /// </summary>
    public ObservableCore Observable => field ??= new();

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>值</returns>
    /// <exception cref="KeyNotFoundException">当键不存在时</exception>
    public TValue GetData(TKey key, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CurrentCulture;
        return DatasByKey[key][_indexByCulture[cultureInfo]];
    }

    /// <summary>
    /// 获取数据或从 <see cref="GetDefaultValue"/> 获取的默认值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>值</returns>
    public TValue GetDataOrDefault(TKey key, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CurrentCulture;
        if (DatasByKey.TryGetValue(key, out var datas) is false)
            return GetDefaultValue(key, cultureInfo);
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return GetDefaultValue(key, cultureInfo);
        return datas[index];
    }

    /// <summary>
    /// 获取数据或默认
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    public TValue GetDataOrDefault(
        TKey key,
        TValue defaultValue = default!,
        CultureInfo? cultureInfo = null
    )
    {
        cultureInfo ??= CurrentCulture;
        if (DatasByKey.TryGetValue(key, out var datas) is false)
            return defaultValue;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return defaultValue;
        return datas[index];
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool SetData(TKey key, TValue value, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        cultureInfo ??= CurrentCulture;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return false;
        if (DatasByKey.TryGetValue(key, out var list) is false)
            list = DatasByKey[key] = new(key);
        list[index] = value;
        return true;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="pairs">数据对</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> 为 <see langword="null"/></exception>
    public void SetDatas(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs,
        CultureInfo? cultureInfo = null
    )
    {
        ArgumentNullException.ThrowIfNull(pairs);
        cultureInfo ??= CurrentCulture;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return;
        foreach (var pair in pairs)
        {
            if (DatasByKey.TryGetValue(pair.Key, out var dic) is false)
                dic = DatasByKey[pair.Key] = new(pair.Key);
            dic[index] = pair.Value;
        }
    }

    /// <summary>
    /// 为为 <see cref="DefaultValue"/> 的值设置数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <returns>是否设置成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool SetDataWhenDefault(TKey key, TValue value, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        cultureInfo ??= CurrentCulture;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return false;
        if (DatasByKey.TryGetValue(key, out var dic) is false)
            dic = DatasByKey[key] = new(key);
        if (ValueComparer.Equals(dic[index], DefaultValue))
        {
            dic[index] = value;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 为为 <see cref="DefaultValue"/> 的值设置数据
    /// </summary>
    /// <param name="pairs">键值对</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> 为 <see langword="null"/></exception>
    public void SetDatasWhenDefault(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs,
        CultureInfo? cultureInfo = null
    )
    {
        ArgumentNullException.ThrowIfNull(pairs);
        cultureInfo ??= CurrentCulture;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return;
        foreach (var pair in pairs)
        {
            if (DatasByKey.TryGetValue(pair.Key, out var dic) is false)
                dic = DatasByKey[pair.Key] = new(pair.Key);
            if (ValueComparer.Equals(dic[index], DefaultValue))
                dic[index] = pair.Value;
        }
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否删除成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
    public bool RemoveData(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        var result = DatasByKey.TryGetValue(key, out var datas);
        if (result)
        {
            DatasByKey.Remove(key);
            datas!.Clear();
        }
        return result;
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public void ClearData()
    {
        DatasByKey.Clear();
    }

    /// <summary>
    /// 复制数据至
    /// </summary>
    /// <param name="resource">资源</param>
    /// <param name="keys">指定键</param>
    public void CopyDataTo(ObservableI18nResource<TKey, TValue> resource, params TKey[] keys)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (keys is null || keys.Length == 0)
        {
            foreach (var (culture, index) in _indexByCulture)
            {
                resource.AddCulture(culture);
                foreach (var datas in DatasByKey.Values)
                {
                    resource.SetData(datas.Key, datas[index], culture);
                }
            }
        }
        else
        {
            foreach (var (culture, index) in _indexByCulture)
            {
                resource.AddCulture(culture);
                foreach (var key in keys)
                {
                    if (DatasByKey.TryGetValue(key, out var datas) is false)
                        continue;
                    resource.SetData(datas.Key, datas[index], culture);
                }
            }
        }
    }

    /// <summary>
    /// 添加文化, 会自动为所有键的新文化添加 <see cref="DefaultValue"/>
    /// </summary>
    /// <param name="cultureInfo">文化</param>
    /// <returns>是否添加成功</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cultureInfo"/> 为 <see langword="null"/></exception>
    public bool AddCulture(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        var result = _indexByCulture.TryAdd(cultureInfo, _indexByCulture.Count);
        if (result)
        {
            foreach (var pair in DatasByKey)
                pair.Value.Add(DefaultValue);
        }
        return result;
    }

    /// <summary>
    /// 删除文化, 禁止删除 <see cref="CurrentCulture"/>
    /// </summary>
    /// <param name="cultureInfo">文化</param>
    /// <returns>是否删除成功</returns>
    /// <exception cref="ArgumentException"><paramref name="cultureInfo"/> == <see cref="CurrentCulture"/></exception>
    /// <exception cref="ArgumentNullException"><paramref name="cultureInfo"/> 为 <see langword="null"/></exception>
    public bool RemoveCulture(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        if (CurrentCulture == cultureInfo)
            throw new ArgumentException(
                "The deleted cultureInfo cannot be the same as CurrentCulture",
                nameof(cultureInfo)
            );
        var result = _indexByCulture.Remove(cultureInfo, out var index);
        if (result)
        {
            foreach (var pair in DatasByKey)
                pair.Value.RemoveAt(index);
            foreach (var (e, i) in _indexByCulture.WithIndex())
                _indexByCulture[e.Key] = i;
        }
        return result;
    }

    /// <summary>
    /// 清除除了 <see cref="CurrentCulture"/> 外的其他文化
    /// </summary>
    public void ClearOtherCulture()
    {
        if (_indexByCulture.Count == 1)
            return;
        var list = _indexByCulture.Where(x => x.Key != CurrentCulture).ToArray();
        for (var i = list.Length - 1; i >= 0; i--)
            _indexByCulture.Remove(list[i].Key);
        foreach (var pair in DatasByKey)
        {
            for (var i = list.Length - 1; i >= 0; i--)
                pair.Value.RemoveAt(list[i].Value);
        }
        _indexByCulture[CurrentCulture] = 0;
    }

    /// <summary>
    /// 清除所有文化
    /// </summary>
    public void ClearCulture()
    {
        if (_indexByCulture.Count == 1)
            return;
        _indexByCulture.Clear();
        foreach (var pair in DatasByKey)
        {
            pair.Value.Clear();
        }
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
        ArgumentNullException.ThrowIfNull(oldKey);
        ArgumentNullException.ThrowIfNull(newKey);
        if (DatasByKey.TryGetValue(oldKey, out var dic) is false)
            return false;
        if (overwrite)
        {
            DatasByKey.Remove(oldKey);
            dic.Key = newKey;
            DatasByKey[newKey] = dic;
            return true;
        }
        else
        {
            var result = DatasByKey.TryAdd(newKey, dic);
            if (result)
            {
                dic.Key = newKey;
                DatasByKey.Remove(oldKey);
            }
            return result;
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
            DatasByKey.DictionaryChanging -= DatasByKey_DictionaryChanging;
            DatasByKey.DictionaryChanged -= DatasByKey_DictionaryChanged;
            Observable.ActionsByKey.Clear();
            ClearData();
            ClearOtherCulture();
        }
        _disposed = true;
    }
    #endregion

    /// <summary>
    /// 文化改变后事件
    /// </summary>
    public event EventHandler<CultureInfo>? CurrentCultureChanged;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 文化数据改变
    /// </summary>
    public event CultureDataChangedEventHandler<TKey, TValue>? CultureDataChanged;

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
    /// 可观察核心
    /// </summary>
    public sealed class ObservableCore()
    {
        internal Dictionary<TKey, List<ValueChangedAction<TKey>>> ActionsByKey { get; } = new();

        /// <summary>
        /// 注册行动
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="getKeyExpression">获取键表达式</param>
        /// <param name="handler">值改变行动</param>
        /// <returns>释放注册的行动</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="getKeyExpression"/> 或 <paramref name="handler"/> 为 <see langword="null"/></exception>
        public IDisposable Action(
            INotifyPropertyChanged source,
            Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
            ValueChangedActionHandler<TKey> handler
        )
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            ArgumentNullException.ThrowIfNull(handler);
            return new ValueChangedAction<TKey>(source, getKeyExpression, handler, ActionsByKey);
        }

        /// <summary>
        /// 清除源注册的所有行动
        /// </summary>
        /// <param name="source">源</param>
        /// <exception cref="ArgumentNullException"> <paramref name="source"/> 为 <see langword="null"/></exception>
        public void ClearActionsBy(INotifyPropertyChanged source)
        {
            ArgumentNullException.ThrowIfNull(source);
            var list = new List<TKey>();
            foreach (var pair in ActionsByKey)
            {
                pair.Value.RemoveAll(action =>
                    source.Equals(action.Source).Action(action, a => a.Dispose(), null)
                );
                if (pair.Value.Count == 0)
                    list.Add(pair.Key);
            }
            for (var i = 0; i < list.Count; i++)
                ActionsByKey.Remove(list[i]);
        }

        /// <summary>
        /// 执行键相关的所有行动
        /// </summary>
        /// <param name="key">键</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/></exception>
        public void DoActionsBy(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            if (ActionsByKey.TryGetValue(key, out var actions))
            {
                foreach (var action in actions)
                    action.Action(action.Source, key);
            }
        }

        /// <summary>
        /// 执行源相关的所有行动
        /// </summary>
        /// <param name="source">源</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <see langword="null"/></exception>
        public void DoActionsBy(INotifyPropertyChanged source)
        {
            ArgumentNullException.ThrowIfNull(source);
            foreach (var pair in ActionsByKey)
            {
                foreach (
                    var action in pair.Value.Where(source, static (x, s) => x.Source.Equals(s))
                )
                    action.Action(action.Source, pair.Key);
            }
        }
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
/// <param name="cultureInfo">文化信息</param>
/// <returns>值</returns>
public delegate TValue GetDefaultCultureDataHandler<in TKey, out TValue>(
    TKey key,
    CultureInfo cultureInfo
)
    where TKey : notnull;

/// <summary>
/// 文化数据改变后事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void CultureDataChangedEventHandler<TKey, TValue>(
    II18nResource sender,
    CultureDataChangedEventArgs<TKey, TValue> e
)
    where TKey : notnull;

/// <summary>
/// I18n观察者
/// </summary>
/// <typeparam name="TObservable">被观察的源对象类型</typeparam>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
#pragma warning disable S2436
public sealed class I18nObserver<TObservable, TKey, TValue>
#pragma warning restore S2436
    : ICollection<I18nObserver<TObservable, TKey, TValue>.ObservedPropertyRegistration>,
        IDisposable
    where TObservable : class, INotifyPropertyChanged
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="resource">I18n资源</param>
    /// <param name="source">观察者源</param>
    /// <param name="notifyPropertyChanged">观察者通知属性改变方法</param>
    public I18nObserver(
        ObservableI18nResource<TKey, TValue> resource,
        TObservable source,
        Action<TObservable, string> notifyPropertyChanged
    )
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(notifyPropertyChanged);
        Resource = resource;
        _source = source;
        _notifyPropertyChanged = notifyPropertyChanged;
        _source.PropertyChanged += Source_PropertyChanged;
    }

    private void Resource_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ObservableI18nResource<TKey, TValue> resource)
            return;
        if (
            string.IsNullOrEmpty(e.PropertyName)
            || e.PropertyName == nameof(resource.CurrentCulture)
        )
        {
            foreach (var data in _datas.Values)
            {
                foreach (var name in data.TargetPropertyNames)
                    _notifyPropertyChanged(_source, name);
            }
        }
    }

    /// <summary>
    /// I18n资源
    /// </summary>
    public ObservableI18nResource<TKey, TValue> Resource
    {
        get => field;
        set
        {
            if (field?.Equals(value) is true)
                return;
            field?.PropertyChanged -= Resource_PropertyChanged;
            field = value;
            field?.PropertyChanged += Resource_PropertyChanged;
        }
    }
    readonly TObservable _source;
    readonly Action<TObservable, string> _notifyPropertyChanged;
    readonly Dictionary<string, ObservedPropertyRegistration> _datas = [];

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetData(TKey, CultureInfo?)"/>
    public TValue GetData(TKey key, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= Resource.CurrentCulture;
        return Resource.DatasByKey[key][Resource.IndexByCulture[cultureInfo]];
    }

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetDataOrDefault(TKey, CultureInfo?)"/>
    public TValue GetDataOrDefault(
        TKey key,
        CultureInfo? cultureInfo = null,
        GetDefaultCultureDataHandler<TKey, TValue>? getDefaultValue = null
    )
    {
        return Resource.GetDataOrDefault(key, cultureInfo);
    }

    /// <summary>
    /// 设置数据并通知
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="cultureInfo">文化信息, 为 <see langword="null"/> 则使用 <see cref="ObservableI18nResource{TKey,TValue}.CurrentCulture"/></param>
    /// <param name="keyPropertyName">键属性名</param>
    /// <returns>是否设置成功</returns>
    public bool SetData(
        TKey key,
        TValue value,
        CultureInfo? cultureInfo = null,
        [CallerArgumentExpression(nameof(key))] string? keyPropertyName = null
    )
    {
        if (Resource.SetData(key, value, cultureInfo) is false || keyPropertyName is null)
            return false;
        var data = _datas[keyPropertyName];
        foreach (var name in data.TargetPropertyNames)
            _notifyPropertyChanged(_source, name);
        return true;
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TObservable source)
            return;
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            foreach (var data in _datas.Values)
            {
                TryUpdateAndNotify(source, data);
            }
        }
        else if (_datas.TryGetValue(e.PropertyName, out var data))
        {
            TryUpdateAndNotify(source, data);
        }

        void TryUpdateAndNotify(TObservable source, ObservedPropertyRegistration data)
        {
            var newKey = data.GetKey(source);
            if (Resource.KeyComparer.Equals(data.OldKey, newKey) is false)
            {
                Resource.RenameKey(data.OldKey, newKey, true);
                data.OldKey = newKey;
                foreach (var name in data.TargetPropertyNames)
                    _notifyPropertyChanged(source, name);
            }
        }
    }

    #region ICollection
    /// <inheritdoc/>
    public int Count => _datas.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// 尝试添加
    /// </summary>
    /// <param name="item">项目</param>
    /// <returns>是否添加成功</returns>
    public bool TryAdd(ObservedPropertyRegistration item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var result = _datas.TryAdd(item.PropertyName, item);
        if (result)
            item.OldKey = item.GetKey(_source);
        return result;
    }

    /// <inheritdoc/>
    public void Add(ObservedPropertyRegistration item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _datas.Add(item.PropertyName, item);
        item.OldKey = item.GetKey(_source);
    }

    /// <inheritdoc/>
    public bool Remove(string propertyName)
    {
        return _datas.Remove(propertyName);
    }

    /// <inheritdoc/>
    bool ICollection<ObservedPropertyRegistration>.Remove(ObservedPropertyRegistration item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _datas.Remove(item.PropertyName);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _datas.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(ObservedPropertyRegistration item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _datas.ContainsKey(item.PropertyName);
    }

    /// <inheritdoc/>
    public void CopyTo(ObservedPropertyRegistration[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        _datas.Values.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<ObservedPropertyRegistration> GetEnumerator()
    {
        return _datas.Values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    #region Dispose
    private bool _disposed;

    /// <inheritdoc/>
    ~I18nObserver() => Dispose(false);

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
            Resource?.PropertyChanged -= Resource_PropertyChanged;
            _source.PropertyChanged -= Source_PropertyChanged;
            _datas.Clear();
        }
        _disposed = true;
    }

    #endregion
    /// <summary>
    /// 源属性注册信息
    /// </summary>
    public sealed class ObservedPropertyRegistration : IEquatable<ObservedPropertyRegistration>
    {
        /// <inheritdoc/>
        /// <param name="getKeyExpression">获取键的表达式, 可同时获取键属性名和获取键方法</param>
        /// <param name="targetPropertyName">目标属性名</param>
        /// <param name="targetPropertyNames">目标属性名</param>
        public ObservedPropertyRegistration(
            Expression<Func<TObservable, TKey>> getKeyExpression,
            string targetPropertyName,
            params string[] targetPropertyNames
        )
        {
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            PropertyName = getKeyExpression.GetPropertyName();
            var getKey = getKeyExpression.Compile();
            GetKey = getKey;
            TargetPropertyNames = [targetPropertyName, .. targetPropertyNames];
        }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// 旧键
        /// </summary>
        public TKey OldKey { get; set; } = default!;

        /// <summary>
        /// 获取键方法
        /// </summary>
        public Func<TObservable, TKey> GetKey { get; }

        /// <summary>
        /// 通知的目标属性
        /// </summary>
        public string[] TargetPropertyNames { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ObservedPropertyRegistration);
        }

        /// <inheritdoc/>
        public bool Equals(ObservedPropertyRegistration? other)
        {
            if (other is null)
                return false;
            return PropertyName == other.PropertyName;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(PropertyName);
        }
    }
}
