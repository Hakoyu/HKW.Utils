using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 枚举命令
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public partial class ObservableEnum<TEnum> : ReactiveObjectX, ICloneable<ObservableEnum<TEnum>>
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

    /// <inheritdoc cref="AddFlagInfo(global::HKW.HKWUtils.IEnumInfo{TEnum})"/>
    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    /// <param name="addFlag">添加枚举值 <c>(v, f) => v | f</c></param>
    /// <param name="removeFlag">删除枚举值 <c>(v, f) => v &amp; ~f</c></param>
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
    /// 是可标记的
    /// </summary>
    public bool IsFlagable => EnumInfo<TEnum>.IsFlagable;

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void AddFlag(TEnum flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        if (AddFlagFunc is null)
            Value = Value.AddFlag(flag);
        else
            Value = AddFlagFunc(Value, flag);
    }

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void AddFlagInfo(IEnumInfo<TEnum> flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        if (AddFlagFunc is null)
            Value = Value.AddFlag(flag.Value);
        else
            Value = AddFlagFunc(Value, flag.Value);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlag(TEnum flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        if (RemoveFlagFunc is null)
            Value = Value.RemoveFlag(flag);
        else
            Value = RemoveFlagFunc(Value, flag);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlagInfo(IEnumInfo<TEnum> flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        if (RemoveFlagFunc is null)
            Value = Value.RemoveFlag(flag.Value);
        else
            Value = RemoveFlagFunc(Value, flag.Value);
    }

    #region ICloneable
    /// <inheritdoc/>
    public ObservableEnum<TEnum> Clone()
    {
        return new(Value);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
    #endregion
    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlag<TEnum> AddFlagFunc { get; set; } = DefaultAddFlagFunc;

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlag<TEnum> RemoveFlagFunc { get; set; } = DefaultRemoveFlagFunc;

    /// <summary>
    /// 默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> DefaultAddFlagFunc { get; set; } = GlobalDefaultAddFlagFunc!;

    /// <summary>
    /// 默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> DefaultRemoveFlagFunc { get; set; } =
        GlobalDefaultRemoveFlagFunc!;

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
/// <returns>添加标志的值</returns>
public delegate TEnum RemoveFlag<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;
