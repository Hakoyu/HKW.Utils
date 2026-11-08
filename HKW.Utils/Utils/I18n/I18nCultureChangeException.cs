using System.Collections.ObjectModel;
using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 文化同步失败阶段.
/// </summary>
public enum I18nCultureChangePhase
{
    /// <summary>
    /// 外部资源 setter 抛错或未保留请求文化, 库内资源和 Core 尚未提交.
    /// </summary>
    ExternalSetter,

    /// <summary>
    /// 资源通知失败, 文化已提交. AddResource 会撤销注册, 但不撤销资源文化.
    /// </summary>
    ResourceNotification,

    /// <summary>
    /// Core 通知失败, 所有文化已提交.
    /// </summary>
    CoreNotification,

    /// <summary>
    /// 执行上下文文化 setter 失败, Core 和库内资源已提交, 上下文可能只完成部分同步.
    /// </summary>
    ContextCulture,
}

/// <summary>
/// 失败时单个资源的实际文化快照. 读取异常非空时文化未知, 不能把 null 当作无当前文化.
/// </summary>
/// <param name="Resource">资源实例</param>
/// <param name="Culture">实际文化</param>
/// <param name="ReadException">读取文化时的异常</param>
public sealed record I18nResourceCultureState(
    II18nResource Resource,
    CultureInfo? Culture,
    Exception? ReadException
);

/// <summary>
/// 文化同步或通知失败, 包含原始异常及失败时实际状态, 不表示全局回滚.
/// </summary>
/// <remarks>
/// 快照只描述捕获时刻, 后续外部写入不会修改快照. 通知异常可能阻止剩余通知投递.
/// </remarks>
public sealed class I18nCultureChangeException : InvalidOperationException
{
    internal I18nCultureChangeException(
        I18nCultureChangePhase phase,
        II18nResource? failedResource,
        CultureInfo? requestedCulture,
        CultureInfo? coreCulture,
        bool coreCommitted,
        IEnumerable<II18nResource> resources,
        Exception innerException
    )
        : base(
            $"Culture synchronization failed during {phase}. Inspect ResourceStates for actual state; no global rollback was performed.",
            innerException
        )
    {
        Phase = phase;
        FailedResource = failedResource;
        RequestedCulture = requestedCulture;
        CoreCulture = coreCulture;
        CoreCommitted = coreCommitted;
        ContextCulture = CultureInfo.CurrentCulture;
        ContextUICulture = CultureInfo.CurrentUICulture;
        var states = new List<I18nResourceCultureState>();
        foreach (var resource in resources)
        {
            try
            {
                states.Add(new(resource, resource.CurrentCulture, null));
            }
            catch (Exception error)
            {
                states.Add(new(resource, null, error));
            }
        }
        ResourceStates = states.AsReadOnly();
    }

    /// <summary>
    /// 失败阶段.
    /// </summary>
    public I18nCultureChangePhase Phase { get; }

    /// <summary>
    /// 引发失败的资源. Core 通知或上下文文化同步失败时为 null.
    /// </summary>
    public II18nResource? FailedResource { get; }

    /// <summary>
    /// 请求的文化.
    /// </summary>
    public CultureInfo? RequestedCulture { get; }

    /// <summary>
    /// 失败时 Core 的实际文化.
    /// </summary>
    public CultureInfo? CoreCulture { get; }

    /// <summary>
    /// 本次切换是否已完成 Core 及库内资源的提交. AddResource 不执行 Core 提交.
    /// </summary>
    public bool CoreCommitted { get; }

    /// <summary>
    /// 失败时执行上下文的实际文化.
    /// </summary>
    public CultureInfo ContextCulture { get; }

    /// <summary>
    /// 失败时执行上下文的实际 UI 文化.
    /// </summary>
    public CultureInfo ContextUICulture { get; }

    /// <summary>
    /// 操作涉及的资源实际状态, 包括 AddResource 失败时未注册的候选资源.
    /// </summary>
    public ReadOnlyCollection<I18nResourceCultureState> ResourceStates { get; }
}
