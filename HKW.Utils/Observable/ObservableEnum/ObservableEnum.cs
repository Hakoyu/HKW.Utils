using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察枚举
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class ObservableEnum<TEnum> : ViewModelBase<ObservableEnum<TEnum>>
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
    public string DisplayValue
    {
        get => _displayValue;
        private set => SetProperty(ref _displayValue, value);
    }
    #endregion
    /// <summary>
    /// 枚举类型
    /// </summary>
    public Type EnumType { get; } = typeof(TEnum);

    /// <summary>
    /// 所有枚举信息
    /// <para>
    /// (EnumType, (EnumName, DisplayAttribute))
    /// </para>
    /// </summary>
    internal static Dictionary<
        Type,
        FrozenDictionary<string, DisplayAttribute>
    > EnumInfos { get; } = new();

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (EnumName, DisplayAttribute)
    /// </para>
    /// </summary>
    public FrozenDictionary<string, DisplayAttribute> EnumInfo { get; }

    /// <inheritdoc/>
    public ObservableEnum()
    {
        if (EnumInfos.TryGetValue(EnumType, out var info))
        {
            EnumInfo = info;
        }
        else
        {
            EnumInfo = EnumInfos[EnumType] = GetEnumInfo<TEnum>();
        }
    }

    /// <inheritdoc/>
    public ObservableEnum(TEnum value)
        : this()
    {
        Value = value;
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public virtual void Refresh()
    {
        DisplayValue = Value.ToString();
    }

    internal static FrozenDictionary<string, DisplayAttribute> GetEnumInfo<T>()
    {
        return Enum.GetNames<TEnum>()
            .Select(static n => (Name: n, FieldInfo: typeof(TEnum).GetField(n)!))
            .Where(static t => t.FieldInfo.IsDefined(typeof(DisplayAttribute)))
            .Select(
                static t =>
                    new KeyValuePair<string, DisplayAttribute>(
                        t.Name,
                        t.FieldInfo.GetCustomAttribute<DisplayAttribute>()!
                    )
            )
            .ToFrozenDictionary();
    }
}
