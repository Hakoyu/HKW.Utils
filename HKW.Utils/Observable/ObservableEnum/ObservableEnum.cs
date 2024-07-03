using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观察枚举
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class ObservableEnum<TEnum> : ReactiveObjectX, IEquatable<ObservableEnum<TEnum>>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public ObservableEnum() { }

    /// <inheritdoc/>
    public ObservableEnum(TEnum value)
        : this()
    {
        Value = value;
    }

    #region Value
    private TEnum _value = default;

    /// <summary>
    /// 枚举值
    /// </summary>
    public TEnum Value
    {
        get => _value;
        set
        {
            // TODO
            //if (SetProperty(ref _value, value))
            //    Refresh();
        }
    }
    #endregion

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 短名称
    /// </summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 全部名称
    /// </summary>
    public static string[] Names { get; } = Enum.GetNames<TEnum>();

    /// <summary>
    /// 全部值
    /// </summary>
    public static TEnum[] Values { get; } = Enum.GetValues<TEnum>();

    /// <summary>
    /// 枚举类型
    /// </summary>
    public static Type EnumType { get; } = typeof(TEnum);

    /// <summary>
    /// 是可标记的
    /// </summary>
    public static bool IsFlagable { get; } = Attribute.IsDefined(EnumType, typeof(FlagsAttribute));

    /// <summary>
    /// 分隔符 (用于显示带有标签的枚举)
    /// </summary>
    [DeniedValues(" | ")]
    public string Separator { get; set; } = " | ";

    /// <summary>
    /// 刷新行动
    /// </summary>
    public Action<ObservableEnum<TEnum>> RefreshAction { get; set; } = DefaultRefreshAction;

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (EnumName, DisplayAttribute)
    /// </para>
    /// </summary>
    public static ImmutableDictionary<TEnum, DisplayAttribute>? EnumInfos { get; private set; } =
        null!;

    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh()
    {
        RefreshAction(this);
    }

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
    public bool HasFlag(TEnum flag)
    {
        return Value.HasFlag(flag);
    }

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="observableEnum">可观察枚举</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
    public bool HasFlag(ObservableEnum<TEnum> observableEnum)
    {
        return Value.HasFlag(observableEnum.Value);
    }

    internal static ImmutableDictionary<TEnum, DisplayAttribute> GetEnumInfo<T>()
    {
        return Values
            .Select(static v =>
                (Value: v, FieldInfo: typeof(TEnum).GetField(Enum.GetName<TEnum>(v)!)!)
            )
            .Where(static v => v.FieldInfo.IsDefined(typeof(DisplayAttribute)))
            .ToImmutableDictionary(
                v => v.Value,
                v => v.FieldInfo.GetCustomAttribute<DisplayAttribute>()!
            );
    }

    #region Equals
    /// <inheritdoc/>
    public bool Equals(ObservableEnum<TEnum>? other)
    {
        return other is null ? false : EqualityComparer<TEnum>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ObservableEnum<TEnum>);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    #endregion

    #region Operator
    /// <inheritdoc/>
    public static bool operator ==(ObservableEnum<TEnum> a, ObservableEnum<TEnum> b)
    {
        return a.Equals(other: b) is true;
    }

    /// <inheritdoc/>
    public static bool operator !=(ObservableEnum<TEnum> a, ObservableEnum<TEnum> b)
    {
        return a.Equals(other: b) is not true;
    }

    ///// <inheritdoc/>
    //public static ObservableEnum<TEnum> operator |(ObservableEnum<TEnum> a, ObservableEnum<TEnum> b)
    //{
    //    if (IsFlagable is false)
    //        throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
    //    return new()
    //    {
    //        RefreshAction = a.RefreshAction,
    //        Separator = a.Separator,
    //        Value = (TEnum)
    //            Enum.ToObject(EnumType, Convert.ToInt64(a.Value) | Convert.ToInt64(b.Value))
    //    };
    //}

    ///// <inheritdoc/>
    //public static ObservableEnum<TEnum> operator &(ObservableEnum<TEnum> a, ObservableEnum<TEnum> b)
    //{
    //    if (IsFlagable is false)
    //        throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
    //    return new()
    //    {
    //        RefreshAction = a.RefreshAction,
    //        Separator = a.Separator,
    //        Value = (TEnum)
    //            Enum.ToObject(EnumType, Convert.ToInt64(a.Value) & Convert.ToInt64(b.Value))
    //    };
    //}

    #endregion

    #region DefaultAction

    /// <summary>
    /// 默认刷新行动
    /// </summary>
    public static Action<ObservableEnum<TEnum>> DefaultRefreshAction { get; } =
        (v) =>
        {
            EnumInfos ??= GetEnumInfo<TEnum>();
            if (IsFlagable)
            {
                var nameSB = new StringBuilder();
                var shortNameSB = new StringBuilder();
                var descriptionSB = new StringBuilder();
                foreach (var value in Values)
                {
                    if (v.Value.HasFlag(value) is false)
                        continue;
                    EnumInfos.TryGetValue(value, out var display);
                    var name = display?.Name ?? value.ToString();
                    nameSB.Append(display?.Name ?? name).Append(v.Separator);
                    shortNameSB.Append(display?.ShortName ?? name).Append(v.Separator);
                    descriptionSB.Append(display?.Description ?? name).Append(v.Separator);
                }
                // 删除多余的分隔符
                nameSB.Remove(nameSB.Length - v.Separator.Length, v.Separator.Length);
                shortNameSB.Remove(nameSB.Length - v.Separator.Length, v.Separator.Length);
                descriptionSB.Remove(nameSB.Length - v.Separator.Length, v.Separator.Length);
                v.Name = nameSB.ToString();
                v.ShortName = shortNameSB.ToString();
                v.Description = descriptionSB.ToString();
            }
            else
            {
                EnumInfos.TryGetValue(v.Value, out var display);
                v.Name = display?.Name ?? v.Value.ToString();
                v.ShortName = display?.ShortName ?? v.Name;
                v.Description = display?.Description ?? v.Name;
            }
        };
    #endregion
}
