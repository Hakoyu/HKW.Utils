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
        AddFlagCommand.ExecuteCommand += AddFlagCommand_ExecuteCommand;
        RemoveFlagCommand.ExecuteCommand += RemoveFlagCommand_ExecuteCommand;
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
    public ObservableCommand<TEnum> AddFlagCommand { get; } = new();

    /// <summary>
    /// 删除枚举标签命令
    /// </summary>
    public ObservableCommand<TEnum> RemoveFlagCommand { get; } = new();

    private void AddFlagCommand_ExecuteCommand(TEnum flag)
    {
        Value = Value.AddFlag(flag);
    }

    private void RemoveFlagCommand_ExecuteCommand(TEnum flag)
    {
        Value = Value.RemoveFlag(flag);
    }
}
