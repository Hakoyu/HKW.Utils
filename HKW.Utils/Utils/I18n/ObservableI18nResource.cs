using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using DynamicData.Binding;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils;

/// <summary>
/// 可观测的I18n资源
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public sealed class ObservableI18nResource<TKey, TValue> : II18nResource, INotifyPropertyChanged
    where TKey : notnull
{
    public ObservableI18nResource(
        string resourceName,
        GetDefaultCultureDataHander<TKey, TValue> getDefault,
        CultureInfo? cultureInfo
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ArgumentNullException.ThrowIfNull(getDefault);
        ResourceName = resourceName;
        GetDefault = getDefault;
        cultureInfo ??= CultureInfo.CurrentCulture;
        _cultures.Add(cultureInfo);
        CurrentCulture = cultureInfo;
        GetCurrentCultureData = new(this);
        GetCurrentCultureDataOrDefault = new(this);
        Observable = new();
        DatasByKey.DictionaryChanging += DatasByKey_DictionaryChanging;
        DatasByKey.DictionaryChanged += DatasByKey_DictionaryChanged;
    }

    private void DatasByKey_DictionaryChanging(
        IObservableDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataDictionary<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Clear)
        {
            foreach (var value in sender.Values)
            {
                value.DictionaryChanged -= Datas_DictionaryChanged;
                value.Clear();
            }
        }
    }

    private void DatasByKey_DictionaryChanged(
        IObservableDictionary<TKey, ObservableCultureDataDictionary<TKey, TValue>> sender,
        NotifyDictionaryChangeEventArgs<TKey, ObservableCultureDataDictionary<TKey, TValue>> e
    )
    {
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair))
            {
                foreach (var culture in _cultures)
                    newPair.Value.TryAdd(culture, DefaultValue);
                newPair.Value.DictionaryChanged += Datas_DictionaryChanged;
            }
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.DictionaryChanged -= Datas_DictionaryChanged;
            }
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetOldPair(out var oldPair))
            {
                oldPair.Value.DictionaryChanged -= Datas_DictionaryChanged;
            }
            if (e.TryGetNewPair(out var newPair))
            {
                foreach (var culture in _cultures)
                    newPair.Value.TryAdd(culture, DefaultValue);
                newPair.Value.DictionaryChanged += Datas_DictionaryChanged;
            }
        }
    }

    private void Datas_DictionaryChanged(
        IObservableDictionary<CultureInfo, TValue> sender,
        NotifyDictionaryChangeEventArgs<CultureInfo, TValue> e
    )
    {
        if (sender is not ObservableCultureDataDictionary<TKey, TValue> dic)
            return;
        CultureInfo? culture = null;
        TValue? oldValue = default!;
        if (e.TryGetOldPair(out var oldPair))
        {
            oldValue = oldPair.Value!;
            culture = oldPair.Key;
        }
        TValue? newValue = default!;
        if (e.TryGetNewPair(out var newPair))
        {
            newValue = newPair.Value!;
            culture = newPair.Key;
        }
        CultureDataChanged?.Invoke(this, new(dic.Key, oldValue, newValue, culture));

        Observable.DoActionsBy(dic.Key);
    }

    public ObservableDictionary<
        TKey,
        ObservableCultureDataDictionary<TKey, TValue>
    > DatasByKey { get; } = new();

    private readonly ObservableSet<CultureInfo> _cultures = new();

    public ReadOnlyObservableSet<CultureInfo> Cultures => field ??= new(_cultures);

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
            ArgumentException.ThrowIfNotContains(_cultures, value);
            field = value;
            CurrentCultureChanged?.Invoke(this, value);
            PropertyChanged?.Invoke(this, new(nameof(CurrentCulture)));
            GetCurrentCultureData?.Refresh();
            GetCurrentCultureDataOrDefault?.Refresh();

            foreach (var key in DatasByKey.Keys)
                Observable.DoActionsBy(key);
        }
    }

    public TValue DefaultValue { get; set; } = default!;

    public GetDefaultCultureDataHander<TKey, TValue> GetDefault { get; set; }

    public GetDataCore GetCurrentCultureData { get; }
    public GetDataOrDefaultCore GetCurrentCultureDataOrDefault { get; }

    public ObservableCore Observable { get; }

    public TValue GetData(TKey key, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CurrentCulture;
        return DatasByKey[key][cultureInfo];
    }

    public TValue GetDataOrDefault(
        TKey key,
        CultureInfo? cultureInfo = null,
        GetDefaultCultureDataHander<TKey, TValue>? getDefault = null
    )
    {
        cultureInfo ??= CurrentCulture;
        getDefault ??= GetDefault;
        if (DatasByKey.TryGetValue(key, out var dic) is false)
            return getDefault(key, cultureInfo);
        if (dic.TryGetValue(cultureInfo, out var value) is false)
            return getDefault(key, cultureInfo);
        return value;
    }

    public bool SetData(TKey key, TValue value, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        cultureInfo ??= CurrentCulture;
        if (_cultures.Contains(cultureInfo) is false)
            return false;
        if (DatasByKey.TryGetValue(key, out var dic) is false)
            dic = DatasByKey[key] = new(key);
        dic[cultureInfo] = value;
        return true;
    }

    public bool SetDataWhenDefault(TKey key, TValue value, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        cultureInfo ??= CurrentCulture;
        if (_cultures.Contains(cultureInfo) is false)
            return false;
        if (DatasByKey.TryGetValue(key, out var dic) is false)
            dic = DatasByKey[key] = new(key);
        if (EqualityComparer<TValue>.Default.Equals(dic[cultureInfo], DefaultValue))
        {
            dic[cultureInfo] = value;
            return true;
        }
        return false;
    }

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

    public void ClearData()
    {
        DatasByKey.Clear();
    }

    public bool AddCulture(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        var result = _cultures.Add(cultureInfo);
        if (result)
        {
            foreach (var pair in DatasByKey)
                pair.Value.Add(cultureInfo, DefaultValue);
        }
        return result;
    }

    public bool RemoveCulture(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        if (CurrentCulture == cultureInfo)
            throw new ArgumentException(
                "The deleted cultureInfo cannot be the same as CurrentCulture",
                nameof(cultureInfo)
            );
        var result = _cultures.Remove(cultureInfo);
        if (result)
        {
            foreach (var pair in DatasByKey)
                pair.Value.Remove(cultureInfo);
        }
        return result;
    }

    public void ClearOtherCulture()
    {
        var array = _cultures.Where(x => x != CurrentCulture).ToArray();
        for (var i = 0; i < array.Length; i++)
            _cultures.Remove(array[i]);
        foreach (var pair in DatasByKey)
        {
            for (var i = 0; i < array.Length; i++)
                pair.Value.Remove(array[i]);
        }
    }

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
            if (Observable.WeakActionsByKey.Remove(oldKey, out var weakActions))
                Observable.WeakActionsByKey.Add(newKey, weakActions);
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
            Observable.WeakActionsByKey.Clear();
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

    public event CultureDataChangedEventHander<TKey, TValue>? CultureDataChanged;

    public class GetDataCore(ObservableI18nResource<TKey, TValue> source) : INotifyPropertyChanged
    {
        public TValue this[TKey key] => source.GetData(key);

        public void Refresh()
        {
            // 刷新 this[]
            PropertyChanged?.Invoke(this, new(""));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class GetDataOrDefaultCore(ObservableI18nResource<TKey, TValue> source)
        : INotifyPropertyChanged
    {
        /// <summary>
        /// 使用 this[] 获取数据或默认
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key] => source.GetDataOrDefault(key);

        /// <summary>
        /// 刷新 this
        /// </summary>
        public void Refresh()
        {
            // 刷新 this[]
            PropertyChanged?.Invoke(this, new(""));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ObservableCore()
    {
        internal Dictionary<TKey, HashSet<ValueChangedAction<TKey>>> ActionsByKey { get; } = new();

        internal Dictionary<TKey, HashSet<WeakValueChangedAction<TKey>>> WeakActionsByKey { get; } =
            new();

        public IDisposable Action(
            INotifyPropertyChanged source,
            Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
            ValueChangedActionHander<TKey> hander
        )
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            ArgumentNullException.ThrowIfNull(hander);
            var action = new ValueChangedAction<TKey>(source, getKeyExpression, hander);
            var disposable = source
                .WhenValueChanged(getKeyExpression)
                .Subscribe(newKey =>
                {
                    if (ActionsByKey.TryGetValue(action.Key, out var oldActions))
                    {
                        oldActions.Remove(action);
                        if (oldActions.Count == 0)
                            ActionsByKey.Remove(action.Key);
                    }

                    action.Key = newKey!;
                    if (ActionsByKey.TryGetValue(newKey!, out var newActions) is false)
                        newActions = ActionsByKey[newKey!] = new();
                    newActions.Add(action);
                });
            action.Disposable = disposable;

            return Disposable.Create(
                (disposable, ActionsByKey, action),
                static x =>
                {
                    if (x.ActionsByKey.TryGetValue(x.action.Key, out var actions))
                    {
                        actions.Remove(x.action);
                        if (actions.Count == 0)
                            x.ActionsByKey.Remove(x.action.Key);
                    }
                    x.disposable.Dispose();
                }
            );
        }

        public IDisposable WeakAction(
            INotifyPropertyChanged source,
            Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
            ValueChangedActionHander<TKey> hander
        )
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            ArgumentNullException.ThrowIfNull(hander);

            var action = new WeakValueChangedAction<TKey>(source, getKeyExpression, hander);
            var disposable = source
                .WhenValueChanged(getKeyExpression)
                .Subscribe(newKey =>
                {
                    if (WeakActionsByKey.TryGetValue(action.Key, out var oldActions))
                    {
                        oldActions.Remove(action);
                        if (oldActions.Count == 0)
                            WeakActionsByKey.Remove(action.Key);
                    }

                    action.Key = newKey!;
                    if (WeakActionsByKey.TryGetValue(newKey!, out var newActions) is false)
                        newActions = WeakActionsByKey[newKey!] = new();
                    newActions.Add(action);
                });
            action.Disposable = disposable;

            return Disposable.Create(
                (disposable, WeakActionsByKey, action),
                static x =>
                {
                    if (x.WeakActionsByKey.TryGetValue(x.action.Key, out var actions))
                    {
                        actions.Remove(x.action);
                        if (actions.Count == 0)
                            x.WeakActionsByKey.Remove(x.action.Key);
                    }
                    x.disposable.Dispose();
                }
            );
        }

        public void ClearInvalidWeakAction()
        {
            var keys = new List<TKey>(WeakActionsByKey.Count);
            var actions = new List<WeakValueChangedAction<TKey>>();
            foreach (var pair in WeakActionsByKey)
            {
                foreach (var action in pair.Value)
                {
                    if (action.Source.TryGetTarget(out _) is false)
                        actions.Add(action);
                }
                pair.Value.ExceptWith(actions);
                actions.Clear();
                if (pair.Value.Count == 0)
                    keys.Add(pair.Key);
            }
            for (var i = 0; i < keys.Count; i++)
                WeakActionsByKey.Remove(keys[i]);
        }

        public void DoActionsBy(TKey key)
        {
            if (ActionsByKey.TryGetValue(key, out var actions))
            {
                foreach (var action in actions)
                    action.Action(action.Source, key);
            }
            if (WeakActionsByKey.TryGetValue(key, out var weakActions))
            {
                var list = new List<WeakValueChangedAction<TKey>>(weakActions.Count);
                foreach (var action in weakActions)
                {
                    if (action.Source.TryGetTarget(out var source))
                        action.Action(source, key);
                    else
                        list.Add(action);
                }
                weakActions.ExceptWith(list);

                if (weakActions.Count == 0)
                    WeakActionsByKey.Remove(key);
            }
        }
    }
}

/// <summary>
/// 文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class ObservableCultureDataDictionary<TKey, TValue>
    : ObservableDictionary<CultureInfo, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; internal set; }

    /// <inheritdoc/>
    public ObservableCultureDataDictionary(TKey key)
    {
        Key = key;
    }
}

public sealed class ValueChangedAction<TKey> : IDisposable
    where TKey : notnull
{
    public ValueChangedAction(
        INotifyPropertyChanged source,
        Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
        ValueChangedActionHander<TKey> action
    )
    {
        PropertyName = getKeyExpression.GetPropertyName();
        GetKey = getKeyExpression.Compile();
        Source = source;
        Action = action;
        Key = GetKey(source);
    }

    public string PropertyName { get; }
    public TKey Key { get; internal set; }
    public Func<INotifyPropertyChanged, TKey> GetKey { get; }
    public INotifyPropertyChanged Source { get; }
    public ValueChangedActionHander<TKey> Action { get; }
    public IDisposable Disposable { get; internal set; }

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ValueChangedAction() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
            Disposable.Dispose();
        _disposed = true;
    }
    #endregion
}

public delegate void ValueChangedActionHander<in TKey>(INotifyPropertyChanged source, TKey key)
    where TKey : notnull;

/// <summary>
/// 获取默认文化数据
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <param name="key">键</param>
/// <param name="cultureInfo">文化信息</param>
/// <returns>值</returns>
public delegate TValue GetDefaultCultureDataHander<in TKey, out TValue>(
    TKey key,
    CultureInfo cultureInfo
)
    where TKey : notnull;

/// <summary>
/// 文化数据改变后事件
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void CultureDataChangedEventHander<TKey, TValue>(
    II18nResource sender,
    CultureDataChangedEventArgs<TKey, TValue> e
)
    where TKey : notnull;

/// <summary>
/// 通知文化数据改变后事件
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class CultureDataChangedEventArgs<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="cultureInfo">文化信息</param>
    /// <param name="key">键</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    public CultureDataChangedEventArgs(
        TKey key,
        TValue? oldValue,
        TValue? newValue,
        CultureInfo? cultureInfo
    )
    {
        CultureInfo = cultureInfo;
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// 文化信息
    /// </summary>
    public CultureInfo? CultureInfo { get; }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// 旧值
    /// </summary>
    public TValue? OldValue { get; }

    /// <summary>
    /// 新值
    /// </summary>
    public TValue? NewValue { get; }
}

public sealed class WeakValueChangedAction<TKey> : IDisposable
    where TKey : notnull
{
    public WeakValueChangedAction(
        INotifyPropertyChanged source,
        Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
        ValueChangedActionHander<TKey> action
    )
    {
        PropertyName = getKeyExpression.GetPropertyName();
        GetKey = getKeyExpression.Compile();
        Source = new(source);
        Action = action;
        Key = GetKey(source);
    }

    public string PropertyName { get; }
    public TKey Key { get; internal set; }
    public Func<INotifyPropertyChanged, TKey> GetKey { get; }
    public WeakReference<INotifyPropertyChanged> Source { get; }
    public ValueChangedActionHander<TKey> Action { get; }
    public IDisposable Disposable { get; internal set; }
    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~WeakValueChangedAction() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
            Disposable.Dispose();
        _disposed = true;
    }
    #endregion
}
