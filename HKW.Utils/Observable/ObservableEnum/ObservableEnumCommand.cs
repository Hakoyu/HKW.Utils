using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察枚举命令
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class ObservableEnumCommand<TEnum> : ObservableEnum<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public ObservableEnumCommand()
    {
        if (Attribute.IsDefined(EnumType, typeof(FlagsAttribute)) is false)
            throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
    }

    /// <inheritdoc/>

    public ObservableEnumCommand(TEnum value)
        : this()
    {
        Value = value;
    }

    /// <summary>
    /// 添加枚举标签命令
    /// </summary>
    [ReactiveCommand]
    private void AddFlag(TEnum flag)
    {
        Value = Value.AddFlag(flag);
    }

    /// <summary>
    /// 删除枚举标签命令
    /// </summary>
    [ReactiveCommand]
    private void RemoveFlag(TEnum flag)
    {
        if (Value.Equals(flag) is false)
            Value = Value.RemoveFlag(flag);
    }
}
