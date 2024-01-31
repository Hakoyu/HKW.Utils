using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察的枚举标签模型
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class ObservableEnumFlags<TEnum> : ViewModelBase<ObservableEnumFlags<TEnum>>
    where TEnum : struct, Enum
{
    #region Value
    private TEnum _value;

    /// <summary>
    /// 枚举值
    /// </summary>
    public TEnum Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
                Refresh();
        }
    }
    #endregion

    #region DisplayValue
    private string _displayValue = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName
    {
        get => _displayValue;
        private set => SetProperty(ref _displayValue, value);
    }
    #endregion

    #region Separator
    private string _separator = " | ";

    /// <summary>
    /// 分隔符 (用于 DisplayName)
    /// </summary>
    [DefaultValue(" | ")]
    public string Separator
    {
        get => _separator;
        set => SetProperty(ref _separator, value);
    }
    #endregion


    /// <summary>
    /// 添加枚举命令
    /// </summary>
    public ObservableCommand<TEnum> AddCommand { get; } = new();

    /// <summary>
    /// 删除枚举命令
    /// </summary>
    public ObservableCommand<TEnum> RemoveCommand { get; } = new();

    /// <summary>
    /// 枚举类型
    /// </summary>
    public Type EnumType = typeof(TEnum);

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (EnumName, DisplayAttribute)
    /// </para>
    /// </summary>
    public FrozenDictionary<string, DisplayAttribute> EnumInfo { get; }

    /// <inheritdoc/>
    /// <exception cref="Exception"></exception>
    public ObservableEnumFlags()
    {
        if (Attribute.IsDefined(EnumType, typeof(FlagsAttribute)) is false)
            throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
        if (ObservableEnum<TEnum>.EnumInfos.TryGetValue(EnumType, out var info))
        {
            EnumInfo = info;
        }
        else
        {
            EnumInfo = ObservableEnum<TEnum>.EnumInfos[EnumType] =
                ObservableEnum<TEnum>.GetEnumInfo<TEnum>();
        }
        AddCommand.ExecuteCommand += AddCommand_ExecuteCommand;
        RemoveCommand.ExecuteCommand += RemoveCommand_ExecuteCommand;
    }

    /// <inheritdoc/>
    /// <param name="value">枚举值</param>
    public ObservableEnumFlags(TEnum value)
        : this()
    {
        Value = value;
    }

    private void AddCommand_ExecuteCommand(TEnum value)
    {
        Value = (TEnum)Enum.ToObject(EnumType, Convert.ToInt64(Value) | Convert.ToInt64(value));
    }

    private void RemoveCommand_ExecuteCommand(TEnum value)
    {
        Value = (TEnum)Enum.ToObject(EnumType, Convert.ToInt64(Value) & ~Convert.ToInt64(value));
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public virtual void Refresh()
    {
        DisplayName = string.Join(
            Separator,
            Enum.GetValues(EnumType).Cast<Enum>().Where(e => Value.HasFlag(e)).ToString()
        );
    }
}
