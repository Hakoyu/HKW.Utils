using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 枚举命令
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
    /// <param name="addFlag">添加枚举值</param>
    /// <param name="removeFlag">删除枚举值</param>
    public ObservableEnum(TEnum value, AddFlag<TEnum> addFlag, RemoveFlag<TEnum> removeFlag)
        : this(value)
    {
        AddFlageFunc = addFlag;
        RemoveFlageFunc = removeFlag;
    }

    /// <summary>
    /// 枚举值
    /// </summary>
    [ReactiveProperty]
    public TEnum Value { get; set; }

    #region IsFlagable
    private static Lazy<bool> _isFlagable =
        new(() => Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute)));

    /// <summary>
    /// 是可标记的
    /// </summary>
    public static bool IsFlagable => _isFlagable.Value;
    #endregion

    /// <summary>
    /// 添加标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void AddFlag(TEnum flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        Value = AddFlageFunc(Value, flag);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlage(TEnum flag)
    {
        if (IsFlagable is false)
            throw new Exception($"This Enum not use attribute \"{nameof(FlagsAttribute)}\".");
        Value = RemoveFlageFunc(Value, flag);
    }

    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlag<TEnum> AddFlageFunc { get; set; } = DefaultAddFlageFunc;

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlag<TEnum> RemoveFlageFunc { get; set; } = DefaultRemoveFlageFunc;

    /// <summary>
    /// 默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> DefaultAddFlageFunc { get; set; } = GlobalDefaultAddFlageFunc!;

    /// <summary>
    /// 默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> DefaultRemoveFlageFunc { get; set; } =
        GlobalDefaultRemoveFlageFunc!;

    /// <summary>
    /// 全局默认添加标志方法
    /// </summary>
    public static AddFlag<TEnum> GlobalDefaultAddFlageFunc { get; } = (v, f) => v.AddFlag(f);

    /// <summary>
    /// 全局默认删除标志方法
    /// </summary>
    public static RemoveFlag<TEnum> GlobalDefaultRemoveFlageFunc { get; } =
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
