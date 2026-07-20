using System.ComponentModel;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测枚举
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public partial class ObservableEnum<TEnum> : ReactiveObjectX
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public ObservableEnum() { }

    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    public ObservableEnum(TEnum value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    /// <param name="addFlag">添加枚举值 <c>(value, flag) => value | flag</c></param>
    /// <param name="removeFlag">删除枚举值 <c>(value, flag) => value &amp; ~flag</c></param>
    public ObservableEnum(TEnum value, AddFlag<TEnum> addFlag, RemoveFlag<TEnum> removeFlag)
        : this(value)
    {
        AddFlagFunc = addFlag;
        RemoveFlagFunc = removeFlag;
    }

    /// <summary>
    /// 枚举值
    /// </summary>
    [ReactiveProperty]
    public TEnum Value { get; set; }

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void AddFlag(TEnum flag)
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(EnumInfo<TEnum>.GetInfo());
        Value = AddFlagFunc(Value, flag);
    }

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void AddFlagInfo(IEnumInfo<TEnum> flag)
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(EnumInfo<TEnum>.GetInfo());
        Value = AddFlagFunc(Value, flag.Value);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlag(TEnum flag)
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(EnumInfo<TEnum>.GetInfo());
        Value = RemoveFlagFunc(Value, flag);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlagInfo(IEnumInfo<TEnum> flag)
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(EnumInfo<TEnum>.GetInfo());
        Value = RemoveFlagFunc(Value, flag.Value);
    }

    private AddFlag<TEnum>? _addFlagFunc;

    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlag<TEnum> AddFlagFunc
    {
        get => _addFlagFunc ?? DefaultAddFlagFunc;
        set => _addFlagFunc = value;
    }
    private RemoveFlag<TEnum>? _removeFlagFunc;

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlag<TEnum> RemoveFlagFunc
    {
        get => _removeFlagFunc ?? DefaultRemoveFlagFunc;
        set => _removeFlagFunc = value;
    }

    private static AddFlag<TEnum>? _defaultAddFlagFunc;

    /// <summary>
    /// 默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> DefaultAddFlagFunc
    {
        get => _defaultAddFlagFunc ?? GlobalDefaultAddFlagFunc;
        set => _defaultAddFlagFunc = value;
    }

    static RemoveFlag<TEnum>? _defaultRemoveFlagFunc;

    /// <summary>
    /// 默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> DefaultRemoveFlagFunc
    {
        get => _defaultRemoveFlagFunc ?? GlobalDefaultRemoveFlagFunc;
        set => _defaultRemoveFlagFunc = value;
    }

    /// <summary>
    /// 全局默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> GlobalDefaultAddFlagFunc { get; } = (v, f) => v.AddFlag(f);

    /// <summary>
    /// 全局默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> GlobalDefaultRemoveFlagFunc { get; } =
        (v, f) => v.RemoveFlag(f);
}

/// <summary>
/// 添加标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>添加标志的值</returns>
public delegate TEnum AddFlag<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;

/// <summary>
/// 删除标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>删除标志的值</returns>
public delegate TEnum RemoveFlag<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;
