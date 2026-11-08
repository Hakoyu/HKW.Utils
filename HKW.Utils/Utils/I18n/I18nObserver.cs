using System.Collections;
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
/// <remarks>
/// 观察键变化只切换引用, 不重命名或覆盖资源. 注册描述可共享, 运行时键由各观察者独立保存于 Registrations.
/// 资源操作开始时捕获全部目标名称, 当前操作不受目标列表后续编辑影响. 非当前文化仅在键的存在性改变时刷新.
/// 同步回调期间允许读取, 拒绝同对象写入和释放, 包括无变化写入. 自身 SetData 的资源通知使用内部核心转发.
/// 调用方须显式 Dispose 解除源和资源的强事件订阅. 不提供跨线程同步或隐式派发.
/// </remarks>
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
        resource.CheckWrite();
        _resource = resource;
        Source = source;
        _notifyPropertyChanged = notifyPropertyChanged;
        Registrations = new ValueCollection(_states);
        Attach(resource);
        Source.PropertyChanged += Source_PropertyChanged;
    }

    private void Resource_CultureDataChanged(
        II18nResource sender,
        CultureDataChangedEventArgs<TKey, TValue> e
    )
    {
        if (ReferenceEquals(sender, Resource) is false || _disposed)
            return;
        var affectsCurrentCulture =
            e.CultureInfo is null || Equals(e.CultureInfo, Resource.CurrentCulture);
        var hasData = affectsCurrentCulture is false && Resource.ContainsData(e.Key);
        foreach (var entry in _snapshot)
        {
            if (
                Resource.KeyComparer.Equals(entry.State.Key, e.Key)
                && (affectsCurrentCulture || entry.HadData != hasData)
            )
                Notify(entry.Names);
        }
    }

    private ObservableI18nResource<TKey, TValue> _resource;
    private bool _writing;
    private bool _updating;
    private string? _forwardedProperty;
    private Snapshot[] _snapshot = [];
    private readonly Dictionary<string, RegistrationState> _states = [];

    private readonly record struct Snapshot(RegistrationState State, string[] Names, bool HadData);

    private sealed class RegistrationState(
        ObservedPropertyRegistration<TObservable, TKey> registration,
        TKey key
    ) : IObservedPropertyRegistration<TKey>
    {
        internal TKey Key = key;
        internal ObservedPropertyRegistration<TObservable, TKey> Registration => registration;
        public string PropertyName => registration.PropertyName;
        public TKey OldKey
        {
            get => Key;
            set => throw new NotSupportedException("Observed keys are managed by the observer.");
        }

        public TKey GetKey(INotifyPropertyChanged source) => registration.GetKey(source);

        public List<string> TargetPropertyNames => registration.TargetPropertyNames;
    }

    private Operation BeginOperation()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Resource.CheckObserverWrite();
        if (_writing)
            throw new InvalidOperationException(
                "Reentrant writes to the same observer are not allowed."
            );
        return CaptureOperation();
    }

    private Operation CaptureOperation()
    {
        var snapshot = new Snapshot[_states.Count];
        var index = 0;
        foreach (var state in _states.Values)
            snapshot[index++] = new(
                state,
                state.TargetPropertyNames.ToArray(),
                Resource.ContainsData(state.Key)
            );
        _writing = true;
        _snapshot = snapshot;
        return new(this);
    }

    private IDisposable CaptureResourceOperation()
    {
        if (_writing)
        {
            // 自身 SetData 使用外层快照, 不重新进入公共写入保护.
            if (_updating)
                return new Operation(null);
            throw new InvalidOperationException(
                "Reentrant writes through an active observer are not allowed."
            );
        }
        return CaptureOperation();
    }

    private readonly struct Operation(I18nObserver<TObservable, TKey, TValue>? owner) : IDisposable
    {
        public void Dispose()
        {
            if (owner is null)
                return;
            owner._snapshot = [];
            owner._writing = false;
        }
    }

    private void Attach(ObservableI18nResource<TKey, TValue> resource)
    {
        resource.RegisterObserver(this);
        resource.ObserverOperationStarting += CaptureResourceOperation;
        resource.CurrentCultureChanged += Resource_CurrentCultureChanged;
        resource.CultureDataChanged += Resource_CultureDataChanged;
    }

    private void Detach(ObservableI18nResource<TKey, TValue> resource)
    {
        resource.ObserverOperationStarting -= CaptureResourceOperation;
        resource.CurrentCultureChanged -= Resource_CurrentCultureChanged;
        resource.CultureDataChanged -= Resource_CultureDataChanged;
        resource.UnregisterObserver(this);
    }

    private void Notify(string[] names)
    {
        foreach (var name in names)
        {
            _forwardedProperty = name;
            try
            {
                _notifyPropertyChanged(Source, name);
            }
            finally
            {
                _forwardedProperty = null;
            }
        }
    }

    /// <summary>
    /// I18n资源
    /// </summary>
    /// <remarks>
    /// 切换前验证新资源并读取所有源键, 提交全部引用后刷新目标.
    /// </remarks>
    public ObservableI18nResource<TKey, TValue> Resource
    {
        get => _resource;
        set
        {
            using var operation = BeginOperation();
            ArgumentNullException.ThrowIfNull(value);
            value.CheckWrite();
            if (ReferenceEquals(_resource, value))
                return;
            var keys = ReadKeys(_snapshot);
            Detach(_resource);
            _resource = value;
            CommitKeys(_snapshot, keys);
            Attach(value);
            foreach (var entry in _snapshot)
                Notify(entry.Names);
        }
    }

    /// <summary>
    /// 获取用于订阅和转发通知的源可观察对象.
    /// </summary>
    public TObservable Source { get; }

    /// <inheritdoc/>
    INotifyPropertyChanged II18nObserver<TKey, TValue>.Source => Source;
    readonly Action<TObservable, string> _notifyPropertyChanged;

    /// <summary>
    /// (PropertyName, Registration)
    /// </summary>
    readonly Dictionary<string, ObservedPropertyRegistration<TObservable, TKey>> _regByName = [];

    /// <inheritdoc/>
    /// <remarks>
    /// 返回本观察者的只读注册视图
    /// </remarks>
    public ICollection<IObservedPropertyRegistration<TKey>> Registrations { get; }

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetData(TKey, CultureInfo?)"/>
    public TValue GetData(TKey key, CultureInfo? culture = null)
    {
        return Resource.GetData(key, culture);
    }

    /// <inheritdoc cref="ObservableI18nResource{TKey,TValue}.GetDataOrDefault(TKey, CultureInfo?)"/>
    public TValue GetDataOrDefault(
        TKey key,
        CultureInfo? culture = null,
        GetDefaultCultureDataHandler<TKey, TValue>? getDefaultValue = null
    )
    {
        if (Resource.TryGetData(key, out var value, culture))
            return value;
        return (getDefaultValue ?? Resource.GetDefaultValue)(
            key,
            culture ?? Resource.CurrentCulture
        );
    }

    /// <summary>
    /// 设置数据并通知
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="culture">文化信息, 为 <see langword="null"/> 则使用 <see cref="ObservableI18nResource{TKey,TValue}.CurrentCulture"/></param>
    /// <param name="keyPropertyName">键属性名</param>
    /// <returns>注册名与键匹配且资源接受写入时为 true, 包括值未变的成功写入</returns>
    /// <remarks>
    /// 是否设置成功由实际资源写入结果决定. 支持 Key, this.Key 或 vm.Key 形式的属性表达式.
    /// 名称为空, 缺少注册或键不匹配时返回 false 且不修改资源; 任意局部变量名不推断为注册名.
    /// 所有同键注册通过资源事件刷新, 无变化写入不额外通知. 资源的通知异常传播, 已提交的新态不回滚.
    /// </remarks>
    public bool SetData(
        TKey key,
        TValue value,
        CultureInfo? culture = null,
        [CallerArgumentExpression(nameof(key))] string? keyPropertyName = null
    )
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(key);
        if (string.IsNullOrWhiteSpace(keyPropertyName))
            return false;
        var name = keyPropertyName[(keyPropertyName.LastIndexOf('.') + 1)..];
        if (
            _states.TryGetValue(name, out var state) is false
            || Resource.KeyComparer.Equals(state.Key, key) is false
        )
            return false;
        _updating = true;
        try
        {
            return Resource.SetData(key, value, culture);
        }
        finally
        {
            _updating = false;
        }
    }

    private void Resource_CurrentCultureChanged(object? sender, CultureInfo? e)
    {
        if (ReferenceEquals(sender, Resource) is false || _disposed)
            return;
        foreach (var entry in _snapshot)
            Notify(entry.Names);
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_disposed || ReferenceEquals(sender, Source) is false)
            return;
        // 目标属性的合成通知不重新进入观察键写入流程.
        if (_forwardedProperty is not null && e.PropertyName == _forwardedProperty)
            return;
        if (
            string.IsNullOrEmpty(e.PropertyName) is false
            && _states.ContainsKey(e.PropertyName) is false
        )
            return;
        // 所属行的键只能由资源提交, 其 Key 通知是当前事务的内部合成更新.
        if (
            _writing
            && Source is ObservableCultureDataList<TKey, TValue> row
            && ReferenceEquals(row.Owner, Resource)
            && e.PropertyName == nameof(row.Key)
        )
        {
            UpdateSourceKeysCore(e.PropertyName);
            return;
        }
        using var operation = BeginOperation();
        UpdateSourceKeysCore(e.PropertyName);
    }

    private void UpdateSourceKeysCore(string? propertyName)
    {
        var entries = string.IsNullOrEmpty(propertyName)
            ? _snapshot
            : _snapshot.Where(entry => entry.State.PropertyName == propertyName).ToArray();
        var keys = ReadKeys(entries);
        var changed = new bool[entries.Length];
        for (var i = 0; i < entries.Length; i++)
            changed[i] = Resource.KeyComparer.Equals(entries[i].State.Key, keys[i]) is false;
        CommitKeys(entries, keys);
        for (var i = 0; i < entries.Length; i++)
        {
            if (changed[i])
                Notify(entries[i].Names);
        }
    }

    private TKey[] ReadKeys(Snapshot[] entries)
    {
        var keys = new TKey[entries.Length];
        for (var i = 0; i < entries.Length; i++)
        {
            var key = entries[i].State.Registration.GetKey(Source);
            ArgumentNullException.ThrowIfNull(key);
            keys[i] = key;
        }
        return keys;
    }

    private static void CommitKeys(Snapshot[] entries, TKey[] keys)
    {
        for (var i = 0; i < entries.Length; i++)
            entries[i].State.Key = keys[i];
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
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(item);
        if (_regByName.ContainsKey(item.PropertyName))
            return false;
        AddCore(item);
        return true;
    }

    /// <inheritdoc/>
    public void Add(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(item);
        AddCore(item);
    }

    private void AddCore(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(item.PropertyName);
        foreach (var name in item.TargetPropertyNames)
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var key = item.GetKey(Source);
        ArgumentNullException.ThrowIfNull(key);
        _regByName.Add(item.PropertyName, item);
        _states.Add(item.PropertyName, new(item, key));
    }

    /// <inheritdoc/>
    public bool Remove(string propertyName)
    {
        using var operation = BeginOperation();
        _states.Remove(propertyName);
        return _regByName.Remove(propertyName);
    }

    /// <inheritdoc/>
    bool ICollection<ObservedPropertyRegistration<TObservable, TKey>>.Remove(
        ObservedPropertyRegistration<TObservable, TKey> item
    )
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(item);
        if (
            _regByName.TryGetValue(item.PropertyName, out var existing) is false
            || ReferenceEquals(existing, item) is false
        )
            return false;
        _states.Remove(item.PropertyName);
        return _regByName.Remove(item.PropertyName);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var operation = BeginOperation();
        _regByName.Clear();
        _states.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(ObservedPropertyRegistration<TObservable, TKey> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _regByName.TryGetValue(item.PropertyName, out var existing)
            && ReferenceEquals(existing, item);
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
            using var operation = BeginOperation();
            Detach(Resource);
            Source.PropertyChanged -= Source_PropertyChanged;
            _regByName.Clear();
            _states.Clear();
        }
        _disposed = true;
    }

    #endregion

    private sealed class ValueCollection(Dictionary<string, RegistrationState> dic)
        : ICollection<IObservedPropertyRegistration<TKey>>
    {
        public int Count => dic.Count;

        public bool IsReadOnly => true;

        void ICollection<IObservedPropertyRegistration<TKey>>.Add(
            IObservedPropertyRegistration<TKey> item
        )
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        bool ICollection<IObservedPropertyRegistration<TKey>>.Remove(
            IObservedPropertyRegistration<TKey> item
        )
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        void ICollection<IObservedPropertyRegistration<TKey>>.Clear()
        {
            throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
        }

        public bool Contains(IObservedPropertyRegistration<TKey> item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return dic.TryGetValue(item.PropertyName, out var state)
                && ReferenceEquals(state, item);
        }

        public void CopyTo(IObservedPropertyRegistration<TKey>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length);
            if (array.Length - arrayIndex < dic.Count)
                throw new ArgumentException(
                    "The destination array has insufficient space.",
                    nameof(array)
                );
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
        ArgumentException.ThrowIfNullOrWhiteSpace(targetPropertyName);
        ArgumentNullException.ThrowIfNull(targetPropertyNames);
        foreach (var name in targetPropertyNames)
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
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
