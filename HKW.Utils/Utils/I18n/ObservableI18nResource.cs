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
    public ObservableI18nResource(string resourceName, CultureInfo? currentCulture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ResourceName = resourceName;
        currentCulture ??= CultureInfo.CurrentCulture;
        _indexByCulture.Add(currentCulture, 0);
        CurrentCulture = currentCulture;
        GetCurrentCultureData = new(this);
        GetCurrentCultureDataOrDefault = new(this);
        Observable = new();
        DatasByKey.DictionaryChanging += DatasByKey_DictionaryChanging;
        DatasByKey.DictionaryChanged += DatasByKey_DictionaryChanged;
    }

    public ObservableI18nResource(
        string resourceName,
        IEnumerable<CultureInfo> cultures,
        CultureInfo currentCulture
    )
        : this(resourceName, currentCulture)
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

    private void DatasByKey_DictionaryChanged(
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

    public ObservableDictionary<TKey, ObservableCultureDataList<TKey, TValue>> DatasByKey { get; } =
        new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly OrderedDictionary<CultureInfo, int> _indexByCulture = new();

    public ReadOnlyDictionary<CultureInfo, int> IndexByCulture => field ??= new(_indexByCulture);

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

    public TValue DefaultValue { get; set; } = default!;

    public GetDefaultCultureDataHandler<TKey, TValue> GetDefaultValue { get; set; } =
        (_, _) => default!;

    public GetDataCore GetCurrentCultureData { get; }
    public GetDataOrDefaultCore GetCurrentCultureDataOrDefault { get; }

    public ObservableCore Observable { get; }

    public TValue GetData(TKey key, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CurrentCulture;
        return DatasByKey[key][_indexByCulture[cultureInfo]];
    }

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

    public bool SetDataWhenDefault(TKey key, TValue value, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        cultureInfo ??= CurrentCulture;
        if (_indexByCulture.TryGetValue(cultureInfo, out var index) is false)
            return false;
        if (DatasByKey.TryGetValue(key, out var dic) is false)
            dic = DatasByKey[key] = new(key);
        if (EqualityComparer<TValue>.Default.Equals(dic[index], DefaultValue))
        {
            dic[index] = value;
            return true;
        }
        return false;
    }

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
            if (EqualityComparer<TValue>.Default.Equals(dic[index], DefaultValue))
                dic[index] = pair.Value;
        }
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
        var result = _indexByCulture.TryAdd(cultureInfo, _indexByCulture.Count);
        if (result)
        {
            foreach (var pair in DatasByKey)
                pair.Value.Add(DefaultValue);
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
            GetCurrentCultureData.Dispose();
            GetCurrentCultureDataOrDefault.Dispose();
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

    public event CultureDataChangedEventHandler<TKey, TValue>? CultureDataChanged;

    public class GetDataCore(ObservableI18nResource<TKey, TValue> source)
        : INotifyPropertyChanged,
            IDisposable
    {
        private ObservableI18nResource<TKey, TValue> _source = source;

        /// <summary>
        /// 使用 this[] 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        /// <exception cref="KeyNotFoundException">未找到键</exception>
        public TValue this[TKey key] => _source.GetData(key);

        public void Refresh()
        {
            // 刷新 this[]
            PropertyChanged?.Invoke(this, new(""));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Dispose
        private bool _disposed;

        /// <inheritdoc/>
        ~GetDataCore() => Dispose(false);

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
                _source = null!;
            }
            _disposed = true;
        }
        #endregion
    }

    public class GetDataOrDefaultCore(ObservableI18nResource<TKey, TValue> source)
        : INotifyPropertyChanged,
            IDisposable
    {
        private ObservableI18nResource<TKey, TValue> _source = source;

        /// <summary>
        /// 使用 this[] 获取数据或默认
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public TValue this[TKey key] => _source.GetDataOrDefault(key);

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

        #region Dispose
        private bool _disposed;

        /// <inheritdoc/>
        ~GetDataOrDefaultCore() => Dispose(false);

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
                _source = null!;
            }
            _disposed = true;
        }
        #endregion
    }

    public class ObservableCore()
    {
        internal Dictionary<TKey, HashSet<ValueChangedAction<TKey>>> ActionsByKey { get; } = new();

        internal Dictionary<TKey, HashSet<WeakValueChangedAction<TKey>>> WeakActionsByKey { get; } =
            new();

        public IDisposable Action(
            INotifyPropertyChanged source,
            Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
            ValueChangedActionHandler<TKey> Handler
        )
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            ArgumentNullException.ThrowIfNull(Handler);
            var action = new ValueChangedAction<TKey>(source, getKeyExpression, Handler);
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

        public void ClearActionsBy(INotifyPropertyChanged source)
        {
            var list = new List<TKey>(ActionsByKey.Count);
            foreach (var pair in ActionsByKey)
            {
                pair.Value.RemoveWhere(action =>
                    source.Equals(action.Source).Action(action, a => a.Disposable.Dispose(), null)
                );
                if (pair.Value.Count == 0)
                    list.Add(pair.Key);
            }
            for (var i = 0; i < list.Count; i++)
                ActionsByKey.Remove(list[i]);
        }

        public IDisposable WeakAction(
            INotifyPropertyChanged source,
            Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
            ValueChangedActionHandler<TKey> Handler
        )
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(getKeyExpression);
            ArgumentNullException.ThrowIfNull(Handler);

            var action = new WeakValueChangedAction<TKey>(source, getKeyExpression, Handler);
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

        public void ClearWeakActionsBy(INotifyPropertyChanged source)
        {
            var keys = new List<TKey>(WeakActionsByKey.Count);
            foreach (var pair in WeakActionsByKey)
            {
                pair.Value.RemoveWhere(action =>
                    action.Source.TryGetTarget(out var s)
                        ? source.Equals(s).Action(action, a => a.Disposable.Dispose(), null)
                        : true.Action(action, a => a.Disposable.Dispose(), null)
                );
                if (pair.Value.Count == 0)
                    keys.Add(pair.Key);
            }
            WeakActionsByKey.RemoveAll(keys);
        }

        public void ClearInvalidWeakAction()
        {
            var keys = new List<TKey>(WeakActionsByKey.Count);
            foreach (var pair in WeakActionsByKey)
            {
                pair.Value.RemoveWhere(action => action.Source.TryGetTarget(out _) is false);
                if (pair.Value.Count == 0)
                    keys.Add(pair.Key);
            }
            WeakActionsByKey.RemoveAll(keys);
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
public class ObservableCultureDataList<TKey, TValue> : ObservableList<TValue>
    where TKey : notnull
{
    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; internal set; }

    /// <inheritdoc/>
    public ObservableCultureDataList(TKey key)
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
        ValueChangedActionHandler<TKey> action
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
    public ValueChangedActionHandler<TKey> Action { get; }
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
        ValueChangedActionHandler<TKey> action
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
    public ValueChangedActionHandler<TKey> Action { get; }
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
