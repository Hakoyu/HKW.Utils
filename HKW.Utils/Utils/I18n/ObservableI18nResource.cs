using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using DynamicData.Binding;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

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
    /// <param name="currentCulture">当前文化</param>
    /// <param name="keyComparer">键比较器</param>
    /// <param name="valueComparer">值比较器</param>
    public ObservableI18nResource(
        string resourceName,
        CultureInfo? currentCulture,
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
        IEnumerable<CultureInfo> cultures,
        CultureInfo currentCulture,
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
    /// 获取默认值, 用于 <see cref="GetDataOrDefault"/> 等参数有 <see cref="GetDefaultCultureDataHandler{TKey, TValue}"/> 的相关操作
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
    /// 可观察的, 提供与 <see cref="INotifyPropertyChanged"/> 的联动接口
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
    /// 获取数据或默认
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="cultureInfo">文化, 为 <see langword="null"/> 时使用 <see cref="CurrentCulture"/></param>
    /// <param name="getDefaultValue">获取默认值</param>
    /// <returns>值</returns>
    public TValue GetDataOrDefault(
        TKey key,
        CultureInfo? cultureInfo = null,
        GetDefaultCultureDataHandler<TKey, TValue>? getDefaultValue = null
    )
    {
        cultureInfo ??= CurrentCulture;
        getDefaultValue ??= GetDefaultValue;
        if (DatasByKey.TryGetValue(key, out var datas) is false)
            return getDefaultValue(key, cultureInfo);
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return getDefaultValue(key, cultureInfo);
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
    /// 重命名键
    /// </summary>
    /// <param name="oldKey">旧键</param>
    /// <param name="newKey">新键</param>
    /// <returns>是否重命名成功</returns>
    public bool RenameKey(TKey oldKey, TKey newKey)
    {
        ArgumentNullException.ThrowIfNull(oldKey);
        ArgumentNullException.ThrowIfNull(newKey);
        if (DatasByKey.TryGetValue(oldKey, out var dic) is false)
            return false;
        var result = DatasByKey.TryAdd(newKey, dic);
        if (result)
        {
            dic.Key = newKey;
            DatasByKey.Remove(oldKey);
            if (Observable.ActionsByKey.Remove(oldKey, out var actions))
                Observable.ActionsByKey.Add(newKey, actions);
        }
        return result;
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
            PropertyChanged?.Invoke(this, new(""));
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
        public TValue this[TKey key] => _source.GetDataOrDefault(key);

        /// <summary>
        /// 刷新 this[]
        /// </summary>
        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new(""));
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
