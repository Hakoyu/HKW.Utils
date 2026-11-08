using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 本地化核心
/// </summary>
public sealed class I18nCore : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Lazy<I18nCore> _lazy = new(() =>
        new I18nCore(CultureInfo.CurrentCulture)
    );

    /// <summary>
    /// 默认 I18nCore
    /// </summary>
    public static I18nCore Default => _lazy.Value;

    /// <summary>
    /// 使用调用上下文的当前文化创建核心.
    /// </summary>
    public I18nCore()
        : this(CultureInfo.CurrentCulture) { }

    /// <inheritdoc/>
    /// <param name="cultureInfo">文化信息, null 表示无当前文化</param>
    public I18nCore(CultureInfo? cultureInfo)
    {
        _currentCulture = cultureInfo;
    }

    private CultureInfo? _currentCulture;
    private bool _writing;
    private bool _disposed;

    /// <summary>
    /// 同步改变的文化类型
    /// </summary>
    [DefaultValue(CultureChangeTypes.None)]
    public CultureChangeTypes CultureChangeTypes
    {
        get;
        set
        {
            using var operation = BeginOperation();
            field = value;
        }
    }

    /// <summary>
    /// 本地化资源字典
    /// <para>(ResourceName, I18nResourceInfo)</para>
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, II18nResource> _resources = [];

    /// <summary>
    /// 本地化资源字典
    /// <para>(ResourceName, I18nResourceInfo)</para>
    /// </summary>
    public ReadOnlyDictionary<string, II18nResource> Resources => field ??= new(_resources);

    /// <summary>
    /// 当前文化, 切换文化会同步改变所有I18n资源的文化
    /// </summary>
    public CultureInfo? CurrentCulture
    {
        get => _currentCulture;
        set
        {
            using var operation = BeginOperation();
            var changing = PropertyChanging;
            var changed = CurrentCultureChanged;
            var propertyChanged = PropertyChanged;
            var resources = _resources.Values.ToArray();
            var changes = new List<(II18nResource Resource, II18nCultureChange Change)>();
            try
            {
                Capture(resources, value, changes);
                Validate(resources, value);
                var cultureChanged = Equals(_currentCulture, value) is false;
                var needsSync = resources.Any(resource =>
                    Equals(resource.CurrentCulture, value) is false
                );
                if (cultureChanged is false && needsSync is false)
                    return;
                changing?.Invoke(this, new(nameof(CurrentCulture)));

                SynchronizeExternal(resources, value);
                foreach (var (_, change) in changes)
                    change.Commit();
                _currentCulture = value;
                try
                {
                    if (value is not null)
                    {
                        if (
                            (CultureChangeTypes & CultureChangeTypes.CultureInfoCurrentCulture) != 0
                        )
                            CultureInfo.CurrentCulture = value;
                        if (
                            (CultureChangeTypes & CultureChangeTypes.CultureInfoCurrentUICulture)
                            != 0
                        )
                            CultureInfo.CurrentUICulture = value;
                    }
                }
                catch (Exception error)
                {
                    throw Failure(
                        I18nCultureChangePhase.ContextCulture,
                        null,
                        value,
                        true,
                        resources,
                        error
                    );
                }

                foreach (var (resource, change) in changes)
                {
                    try
                    {
                        change.Publish();
                    }
                    catch (Exception error)
                    {
                        throw Failure(
                            I18nCultureChangePhase.ResourceNotification,
                            resource,
                            value,
                            true,
                            resources,
                            error
                        );
                    }
                }
                if (cultureChanged)
                {
                    try
                    {
                        changed?.Invoke(this, value);
                        propertyChanged?.Invoke(this, new(nameof(CurrentCulture)));
                    }
                    catch (Exception error)
                    {
                        throw Failure(
                            I18nCultureChangePhase.CoreNotification,
                            null,
                            value,
                            true,
                            resources,
                            error
                        );
                    }
                }
            }
            finally
            {
                Release(changes);
            }
        }
    }

    /// <summary>
    /// 文化改变后事件
    /// </summary>
    public event EventHandler<CultureInfo?>? CurrentCultureChanged;

    /// <summary>
    /// 属性改变前事件
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 添加资源
    /// </summary>
    /// <param name="resource">资源</param>
    /// <returns>是否成功添加</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resource"/> 为 <see langword="null"/></exception>
    /// <remarks>
    /// 同步成功后才注册. 库内资源通知时注册已可见, 通知失败会移除该注册, 但保留资源已提交的文化.
    /// 失败不会释放候选资源. 外部 setter 可能先提交后抛错, 失败快照包含未注册的候选资源.
    /// </remarks>
    public bool AddResource(II18nResource resource)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(resource);
        var name = resource.ResourceName;
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (
            _resources.ContainsKey(name)
            || _resources.Values.Any(item => ReferenceEquals(item, resource))
        )
            return false;
        var resources = _resources.Values.Append(resource).ToArray();
        var changes = new List<(II18nResource Resource, II18nCultureChange Change)>();
        try
        {
            // 已注册资源只冻结原态, Add 不顺带修复调用方独立修改的资源文化.
            foreach (var current in _resources.Values.OfType<II18nCultureParticipant>())
            {
                var registered = (II18nResource)current;
                changes.Add((registered, current.CaptureCultureChange(registered.CurrentCulture)));
            }
            II18nCultureChange? candidate = null;
            if (resource is II18nCultureParticipant participant)
            {
                candidate = participant.CaptureCultureChange(_currentCulture);
                changes.Add((resource, candidate));
            }
            Validate([resource], _currentCulture);
            if (candidate is null)
                SynchronizeExternal([resource], _currentCulture, resources);
            candidate?.Commit();
            _resources.Add(name, resource);
            try
            {
                candidate?.Publish();
            }
            catch (Exception error)
            {
                _resources.Remove(name);
                throw Failure(
                    I18nCultureChangePhase.ResourceNotification,
                    resource,
                    _currentCulture,
                    false,
                    resources,
                    error
                );
            }
            return true;
        }
        finally
        {
            Release(changes);
        }
    }

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="resource">资源</param>
    /// <returns>是否成功删除</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resource"/> 为 <see langword="null"/></exception>
    public bool RemoveResource(II18nResource resource)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(resource);
        foreach (var pair in _resources)
        {
            if (ReferenceEquals(pair.Value, resource))
                return _resources.Remove(pair.Key);
        }
        return false;
    }

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="resourceName">资源名称</param>
    /// <returns>是否成功删除</returns>
    public bool RemoveResource(string resourceName)
    {
        using var operation = BeginOperation();
        return _resources.Remove(resourceName);
    }

    /// <summary>
    /// 清空资源
    /// </summary>
    public void ClearResource()
    {
        using var operation = BeginOperation();
        _resources.Clear();
    }

    /// <summary>
    /// 解除全部注册和 Core 事件, 不释放资源或观察器.
    /// </summary>
    public void Dispose()
    {
        if (_writing)
            throw new InvalidOperationException(
                "Reentrant writes to the same core are not allowed."
            );
        if (_disposed)
            return;
        _disposed = true;
        _resources.Clear();
        CurrentCultureChanged = null;
        PropertyChanging = null;
        PropertyChanged = null;
    }

    private Operation BeginOperation()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_writing)
            throw new InvalidOperationException(
                "Reentrant writes to the same core are not allowed."
            );
        _writing = true;
        return new(this);
    }

    private readonly struct Operation(I18nCore core) : IDisposable
    {
        public void Dispose() => core._writing = false;
    }

    private static void Capture(
        II18nResource[] resources,
        CultureInfo? culture,
        List<(II18nResource Resource, II18nCultureChange Change)> changes
    )
    {
        foreach (var resource in resources.OfType<II18nCultureParticipant>())
            changes.Add(((II18nResource)resource, resource.CaptureCultureChange(culture)));
    }

    private static void Validate(II18nResource[] resources, CultureInfo? culture)
    {
        if (culture is null)
            return;
        foreach (var resource in resources)
        {
            if (resource.Cultures.Contains(culture) is false)
                throw new ArgumentException(
                    "The requested culture is not supported by every resource.",
                    nameof(culture)
                );
        }
    }

    private void SynchronizeExternal(
        II18nResource[] resources,
        CultureInfo? culture,
        II18nResource[]? allResources = null
    )
    {
        var external = resources
            .Where(resource => resource is not II18nCultureParticipant)
            .ToArray();
        foreach (var resource in external)
        {
            try
            {
                if (Equals(resource.CurrentCulture, culture) is false)
                    resource.CurrentCulture = culture;
                if (Equals(resource.CurrentCulture, culture) is false)
                    throw new InvalidOperationException(
                        "An external resource did not retain the requested culture."
                    );
            }
            catch (Exception error)
            {
                throw Failure(
                    I18nCultureChangePhase.ExternalSetter,
                    resource,
                    culture,
                    false,
                    allResources ?? resources,
                    error
                );
            }
        }
        foreach (var resource in external)
        {
            try
            {
                if (Equals(resource.CurrentCulture, culture) is false)
                    throw new InvalidOperationException(
                        "An external resource did not retain the requested culture."
                    );
            }
            catch (Exception error)
            {
                throw Failure(
                    I18nCultureChangePhase.ExternalSetter,
                    resource,
                    culture,
                    false,
                    allResources ?? resources,
                    error
                );
            }
        }
    }

    private I18nCultureChangeException Failure(
        I18nCultureChangePhase phase,
        II18nResource? resource,
        CultureInfo? culture,
        bool committed,
        II18nResource[] resources,
        Exception error
    ) => new(phase, resource, culture, _currentCulture, committed, resources, error);

    private static void Release(List<(II18nResource Resource, II18nCultureChange Change)> changes)
    {
        for (var i = changes.Count - 1; i >= 0; i--)
            changes[i].Change.Dispose();
    }
}
