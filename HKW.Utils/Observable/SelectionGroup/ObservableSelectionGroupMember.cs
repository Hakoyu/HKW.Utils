using System.Diagnostics;
using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中组成员
/// </summary>
[DebuggerDisplay("IsSelected = {IsSelected}")]
public partial class ObservableSelectionGroupMember : ReactiveObjectX
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    public ObservableSelectionGroupMember(object source = null!)
    {
        Source = source;
        Wrapper = new(this, nameof(IsSelected), x => x.IsSelected, (x, v) => x.IsSelected = v);
    }

    /// <summary>
    /// 已选中
    /// </summary>
    [ReactiveProperty]
    public bool IsSelected { get; set; }

    /// <summary>
    /// 源
    /// </summary>
    [ReactiveProperty]
    public object Source { get; set; }

    /// <summary>
    /// 包装器
    /// </summary>
    public ObservablePropertyWrapper<ObservableSelectionGroupMember, bool> Wrapper { get; }
}

/// <summary>
/// 可观测可选中组成员
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
public partial class ObservableSelectionGroupMember<TSource>
    : ReactiveObjectX,
        IEquatable<ObservableSelectionGroupMember<TSource>>
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    public ObservableSelectionGroupMember(TSource source = default!)
    {
        Source = source;
        Wrapper = new(this, nameof(IsSelected), x => x.IsSelected, (x, v) => x.IsSelected = v);
    }

    /// <summary>
    /// 已选中
    /// </summary>
    [ReactiveProperty]
    public bool IsSelected { get; set; }

    /// <summary>
    /// 源
    /// </summary>
    [ReactiveProperty]
    public TSource Source { get; set; }

    /// <summary>
    /// 包装器
    /// </summary>
    public ObservablePropertyWrapper<ObservableSelectionGroupMember<TSource>, bool> Wrapper { get; }

    /// <inheritdoc/>
    public override string? ToString()
    {
        return Source?.ToString();
    }

    #region IEquatable
    /// <inheritdoc/>
    public bool Equals(ObservableSelectionGroupMember<TSource>? other)
    {
        if (other is null)
            return false;
        return Source?.Equals(other.Source) is true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableSelectionGroupMember<TSource>);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Source?.GetHashCode() ?? 0;
    }
    #endregion
}
