using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils;

/// <summary>
/// I18n观察者接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public interface II18nObserver<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 源
    /// </summary>
    public INotifyPropertyChanged Source { get; }

    /// <summary>
    /// 资源
    /// </summary>
    public ObservableI18nResource<TKey, TValue> Resource { get; set; }

    /// <summary>
    /// 注册
    /// </summary>
    public ICollection<IObservedPropertyRegistration<TKey>> Registrations { get; }
}

/// <summary>
/// I18n观察者
/// </summary>
/// <typeparam name="TObservable">被观察的源对象类型</typeparam>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
#pragma warning disable S2436
[DebuggerDisplay("ResourceName = {Resource.Name}, Count = {_regByName.Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class I18nObserver<TObservable, TKey, TValue>
#pragma warning restore S2436
    : ICollection<ObservedPropertyRegistration<TObservable, TKey>>,
        II18nObserver<TKey, TValue>,
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
        Source = source;
        _notifyPropertyChanged = notifyPropertyChanged;
        Registrations = new ValueCollection(_regByName);
        Source.PropertyChanged += Source_PropertyChanged;
        if (Resource.RecordObserver)
            Resource.Observers.Add(this);
    }

    private void Resource_CultureDataChanged(
        II18nResource sender,
        CultureDataChangedEventArgs<TKey, TValue> e
    )
    {
        if (Resource.DatasByKey.Comparer.Equals(_updatingKey, e.Key))
            return;
        foreach (var reg in _regByName.Values)
        {
            if (Resource.DatasByKey.Comparer.Equals(reg.OldKey, e.Key) is false)
                continue;
            foreach (var name in reg.TargetPropertyNames)
                _notifyPropertyChanged(Source, name);
        }
    }

    private TKey? _updatingKey;

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
            field?.CurrentCultureChanged -= Resource_CurrentCultureChanged;
            field?.CultureDataChanged -= Resource_CultureDataChanged;
            field?.Observers.Remove(this);
            field = value;
            if (field?.RecordObserver is true)
                field.Observers.Add(this);
            field?.CurrentCultureChanged += Resource_CurrentCultureChanged;
            field?.CultureDataChanged += Resource_CultureDataChanged;
        }
    }

    /// <summary>
    /// 获取用于订阅和转发通知的源可观察对象。
    /// </summary>
    /// <remarks>只读属性，返回用于观察序列的可观察对象实例。对返回对象的更改不会更改此属性的引用，调用方不应假定返回值为可变或线程安全。</remarks>
    public TObservable Source { get; }

    /// <inheritdoc/>
    INotifyPropertyChanged II18nObserver<TKey, TValue>.Source => Source;
    readonly Action<TObservable, string> _notifyPropertyChanged;

    /// <summary>
    /// (PropertyName, Registration)
    /// </summary>
    readonly Dictionary<string, ObservedPropertyRegistration<TObservable, TKey>> _regByName = [];

    /// <inheritdoc/>
    public ICollection<IObservedPropertyRegistration<TKey>> Registrations { get; }

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetData(TKey, CultureInfo?)"/>
    public TValue GetData(TKey key, CultureInfo? culture = null)
    {
        culture ??= Resource.CurrentCulture;
        return Resource.DatasByKey[key][Resource.IndexByCulture[culture]];
    }

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetDataOrDefault(TKey, CultureInfo?)"/>
    public TValue GetDataOrDefault(
        TKey key,
        CultureInfo? culture = null,
        GetDefaultCultureDataHandler<TKey, TValue>? getDefaultValue = null
    )
    {
        return Resource.GetDataOrDefault(key, culture);
    }

    /// <summary>
    /// 设置数据并通知
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="culture">文化信息, 为 <see langword="null"/> 则使用 <see cref="ObservableI18nResource{TKey,TValue}.CurrentCulture"/></param>
    /// <param name="keyPropertyName">键属性名</param>
    /// <returns>是否设置成功</returns>
    public bool SetData(
        TKey key,
        TValue value,
        CultureInfo? culture = null,
        [CallerArgumentExpression(nameof(key))] string? keyPropertyName = null
    )
    {
        _updatingKey = key;
        try
        {
            if (Resource.SetData(key, value, culture) is false || keyPropertyName is null)
                return false;
            var data = _regByName[keyPropertyName];
            foreach (var name in data.TargetPropertyNames)
                _notifyPropertyChanged(Source, name);
        }
        finally
        {
            _updatingKey = default;
        }
        return true;
    }

    private void Resource_CurrentCultureChanged(object? sender, CultureInfo e)
    {
        foreach (var data in _regByName.Values)
        {
            foreach (var name in data.TargetPropertyNames)
                _notifyPropertyChanged(Source, name);
        }
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TObservable source)
            return;
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            foreach (var data in _regByName.Values)
            {
                TryUpdateAndNotify(source, data);
            }
        }
        else if (_regByName.TryGetValue(e.PropertyName, out var data))
        {
            TryUpdateAndNotify(source, data);
        }

        void TryUpdateAndNotify(
            TObservable source,
            ObservedPropertyRegistration<TObservable, TKey> data
        )
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
    public int Count => _regByName.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// 尝试添加
    /// </summary>
    /// <param name="item">项目</param>
    /// <returns>是否添加成功</returns>
    public bool TryAdd(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var result = _regByName.TryAdd(item.PropertyName, item);
        if (result)
            item.OldKey = item.GetKey(Source);
        return result;
    }

    /// <inheritdoc/>
    public void Add(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _regByName.Add(item.PropertyName, item);
        item.OldKey = item.GetKey(Source);
    }

    /// <inheritdoc/>
    public bool Remove(string propertyName)
    {
        return _regByName.Remove(propertyName);
    }

    /// <inheritdoc/>
    bool ICollection<ObservedPropertyRegistration<TObservable, TKey>>.Remove(
        ObservedPropertyRegistration<TObservable, TKey> item
    )
    {
        ArgumentNullException.ThrowIfNull(item);

        return _regByName.Remove(item.PropertyName);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _regByName.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _regByName.ContainsKey(item.PropertyName);
    }

    /// <inheritdoc/>
    public void CopyTo(ObservedPropertyRegistration<TObservable, TKey>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        _regByName.Values.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<ObservedPropertyRegistration<TObservable, TKey>> GetEnumerator()
    {
        return _regByName.Values.GetEnumerator();
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
            Resource?.CurrentCultureChanged -= Resource_CurrentCultureChanged;
            Resource?.CultureDataChanged -= Resource_CultureDataChanged;
            Resource?.Observers.Remove(this);
            Source.PropertyChanged -= Source_PropertyChanged;
        }
        _disposed = true;
    }

    #endregion

    private sealed class ValueCollection(
        Dictionary<string, ObservedPropertyRegistration<TObservable, TKey>> dic
    ) : ICollection<IObservedPropertyRegistration<TKey>>
    {
        public int Count => dic.Count;

        public bool IsReadOnly => true;

        void ICollection<IObservedPropertyRegistration<TKey>>.Add(
            IObservedPropertyRegistration<TKey> item
        )
        {
            throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<IObservedPropertyRegistration<TKey>>.Remove(
            IObservedPropertyRegistration<TKey> item
        )
        {
            throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<IObservedPropertyRegistration<TKey>>.Clear()
        {
            throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
        }

        public bool Contains(IObservedPropertyRegistration<TKey> item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return dic.ContainsKey(item.PropertyName);
        }

        public void CopyTo(IObservedPropertyRegistration<TKey>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            foreach (var registration in dic.Values)
                array[arrayIndex++] = registration;
        }

        public IEnumerator<IObservedPropertyRegistration<TKey>> GetEnumerator()
        {
            return dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

/// <summary>
/// 源属性注册信息接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
public interface IObservedPropertyRegistration<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// 旧键
    /// </summary>
    public TKey OldKey { get; set; }

    /// <summary>
    /// 获取键
    /// </summary>
    public TKey GetKey(INotifyPropertyChanged source);

    /// <summary>
    ///
    /// </summary>
    public List<string> TargetPropertyNames { get; }
}

/// <summary>
/// 源属性注册信息
/// </summary>
/// <typeparam name="TObservable">被观察的源对象类型</typeparam>
/// <typeparam name="TKey">键类型</typeparam>
public sealed class ObservedPropertyRegistration<TObservable, TKey>
    : IObservedPropertyRegistration<TKey>
    where TObservable : class, INotifyPropertyChanged
    where TKey : notnull
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
        _getKey = getKey;
        TargetPropertyNames = [targetPropertyName, .. targetPropertyNames];
    }

    /// <inheritdoc/>
    public string PropertyName { get; }

    /// <inheritdoc/>
    public TKey OldKey { get; set; } = default!;

    private readonly Func<TObservable, TKey> _getKey;

    /// <summary>
    /// 获取键
    /// </summary>
    public TKey GetKey(TObservable source)
    {
        return _getKey(source);
    }

    /// <inheritdoc/>
    public TKey GetKey(INotifyPropertyChanged source)
    {
        return GetKey((TObservable)source);
    }

    /// <inheritdoc/>
    public List<string> TargetPropertyNames { get; }
}
