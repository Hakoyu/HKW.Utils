using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 枚举命令
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public partial class EnumCommand<TEnum> : ReactiveObjectX
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public EnumCommand()
    {
        if (Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute)) is false)
            throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
    }

    /// <inheritdoc/>
    /// <param name="enum">枚举值</param>
    public EnumCommand(TEnum @enum)
    {
        Value = @enum;
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
        Value = AddFlageFunc(Value, flag);
    }

    /// <summary>
    /// 删除标志
    /// </summary>
    /// <param name="flag">标志</param>
    [ReactiveCommand]
    public void RemoveFlage(TEnum flag)
    {
        Value = RemoveFlageFunc(Value, flag);
    }

    /// <summary>
    /// 添加标志方法
    /// </summary>
    public AddFlage<TEnum> AddFlageFunc { get; set; } = DefaultAddFlageFunc;

    /// <summary>
    /// 删除标志方法
    /// </summary>
    public RemoveFlage<TEnum> RemoveFlageFunc { get; set; } = DefaultRemoveFlageFunc;

    /// <summary>
    /// 默认添加标志方法
    /// </summary>
    public static AddFlage<TEnum> DefaultAddFlageFunc { get; set; } = GlobalDefaultAddFlageFunc!;

    /// <summary>
    /// 默认删除标志方法
    /// </summary>
    public static RemoveFlage<TEnum> DefaultRemoveFlageFunc { get; set; } =
        GlobalDefaultRemoveFlageFunc!;

    /// <summary>
    /// 全局默认添加标志方法
    /// </summary>
    public static AddFlage<TEnum> GlobalDefaultAddFlageFunc { get; } = (v, f) => v.AddFlag(f);

    /// <summary>
    /// 全局默认删除标志方法
    /// </summary>
    public static RemoveFlage<TEnum> GlobalDefaultRemoveFlageFunc { get; } =
        (v, f) => v.RemoveFlag(f);
}

/// <summary>
/// 添加标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>添加标志的值</returns>
public delegate TEnum AddFlage<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;

/// <summary>
/// 删除标志
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
/// <param name="value">值</param>
/// <param name="flag">标志</param>
/// <returns>添加标志的值</returns>
public delegate TEnum RemoveFlage<TEnum>(TEnum value, TEnum flag)
    where TEnum : struct, Enum;
