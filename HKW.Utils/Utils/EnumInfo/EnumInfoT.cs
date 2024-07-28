using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

/// <summary>
/// 可观察枚举
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class EnumInfo<TEnum> : IEnumInfo<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    protected EnumInfo() { }

    private EnumInfo(TEnum value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public TEnum Value { get; }
    Enum IEnumInfo.Value => Value;

    /// <inheritdoc/>
    public string Name => GetName(this);

    /// <inheritdoc/>
    public string ShortName => GetShortName(this);

    /// <inheritdoc/>
    public string Description => GetDescription(this);

    /// <inheritdoc/>
    public DisplayAttribute? Display =>
        EnumDisplays is null ? null : EnumDisplays!.GetValueOrDefault(Value, defaultValue: null);

    /// <summary>
    /// 获取名称
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetName { get; set; } = DefaultGetName;

    /// <summary>
    /// 获取短名称
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetShortName { get; set; } = DefaultGetShortName;

    /// <summary>
    /// 获取描述
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetDescription { get; set; } = DefaultGetDescription;

    /// <summary>
    /// 到字符串行动
    /// </summary>
    public Func<EnumInfo<TEnum>, string> ToStringFunc { get; set; } = DefaultToString;

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
    /// <param name="flag">可观察枚举</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
    public bool HasFlag(EnumInfo<TEnum> flag)
    {
        return Value.HasFlag(flag.Value);
    }

    /// <summary>
    /// 获取标志
    /// </summary>
    /// <returns>全部标志</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public IEnumerable<TEnum> GetFlags()
    {
        if (IsFlagable is false)
            throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
        return Values.Where(i => Value.HasFlag(i));
    }

    /// <summary>
    /// 获取标志信息
    /// </summary>
    /// <returns>全部标志信息</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public IEnumerable<EnumInfo<TEnum>> GetFlagInfos()
    {
        if (IsFlagable is false)
            throw new Exception($"此枚举类型未使用特性 \"{nameof(FlagsAttribute)}\"");
        return Values.Cast<EnumInfo<TEnum>>().Where(i => Value.HasFlag(i));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return ToStringFunc(this);
    }

    #region IEquatable
    /// <inheritdoc/>
    public bool Equals(IEnumInfo<TEnum>? other)
    {
        if (other is null)
            return false;
        return Value.Equals(other.Value);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IEnumInfo<TEnum>);
    }

    /// <inheritdoc/>
    public bool Equals(TEnum other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    #endregion

    #region Operator
    /// <inheritdoc/>
    public static implicit operator TEnum(EnumInfo<TEnum> info)
    {
        return info.Value;
    }

    /// <inheritdoc/>
    public static bool operator ==(EnumInfo<TEnum> a, TEnum b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(EnumInfo<TEnum> a, TEnum b)
    {
        return a.Equals(b) is not true;
    }

    /// <inheritdoc/>
    public static bool operator ==(TEnum a, EnumInfo<TEnum> b)
    {
        return a.Equals(b.Value);
    }

    /// <inheritdoc/>
    public static bool operator !=(TEnum a, EnumInfo<TEnum> b)
    {
        return a.Equals(b.Value) is not true;
    }

    /// <inheritdoc/>
    public static bool operator ==(EnumInfo<TEnum> a, EnumInfo<TEnum> b)
    {
        return a.Equals(other: b.Value) is true;
    }

    /// <inheritdoc/>
    public static bool operator !=(EnumInfo<TEnum> a, EnumInfo<TEnum> b)
    {
        return a.Equals(other: b.Value) is not true;
    }
    #endregion



    #region static

    /// <summary>
    /// 初始化, 初始化枚举信息并存储至 <see cref="EnumInfo"/>
    /// </summary>
    public static void Initialize()
    {
        EnumInfo.InfosByType.TryAdd(EnumType, Infos);
    }

    private static FrozenDictionary<Enum, IEnumInfo>? _infos;

    /// <summary>
    /// 信息
    /// </summary>
    public static FrozenDictionary<Enum, IEnumInfo> Infos =>
        _infos ??= Values.ToFrozenDictionary(v => (Enum)v, v => (IEnumInfo)new EnumInfo<TEnum>(v));

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="enum">枚举值</param>
    /// <returns>信息</returns>
    public static EnumInfo<TEnum> GetInfo(TEnum @enum)
    {
        return (EnumInfo<TEnum>)Infos[@enum];
    }

    #region Names
    private static Lazy<FrozenSet<string>> _names = new(() => Enum.GetNames<TEnum>().ToFrozenSet());

    /// <summary>
    /// 全部名称
    /// </summary>
    public static FrozenSet<string> Names => _names.Value;
    #endregion

    #region Values
    private static Lazy<FrozenSet<TEnum>> _values =
        new(() => Enum.GetValues<TEnum>().ToFrozenSet());

    /// <summary>
    /// 全部值
    /// </summary>
    public static FrozenSet<TEnum> Values => _values.Value;
    #endregion

    /// <summary>
    /// 枚举类型
    /// </summary>
    public static Type EnumType { get; } = typeof(TEnum);

    #region IsFlagable
    private static Lazy<bool> _isFlagable =
        new(() => Attribute.IsDefined(EnumType, typeof(FlagsAttribute)));

    /// <summary>
    /// 是可标记的
    /// </summary>
    public static bool IsFlagable => _isFlagable.Value;
    #endregion

    #region EnumDisplays
    private static Lazy<FrozenDictionary<TEnum, DisplayAttribute>> _enumDisplays =
        new(() => GetEnumInfo());

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (Enum, DisplayAttribute)
    /// </para>
    /// </summary>
    public static FrozenDictionary<TEnum, DisplayAttribute> EnumDisplays => _enumDisplays.Value;

    internal static FrozenDictionary<TEnum, DisplayAttribute> GetEnumInfo()
    {
        return Values
            .Select(static v => (Value: v, FieldInfo: typeof(TEnum).GetField(v.ToString())!))
            .Where(static v => v.FieldInfo.IsDefined(typeof(DisplayAttribute)))
            .ToFrozenDictionary(
                v => v.Value,
                v => v.FieldInfo.GetCustomAttribute<DisplayAttribute>()!
            );
    }
    #endregion

    #region Default

    #region DefaultToString
    private static Func<EnumInfo<TEnum>, string>? _defaultToString;

    /// <summary>
    /// 默认到字符串方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultToString
    {
        get => _defaultToString ?? GlobalDefaultToString;
        set => _defaultToString = value;
    }
    #endregion

    #region DefaultGetName
    private static Func<EnumInfo<TEnum>, string>? _defaultGetName;

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetName
    {
        get => _defaultGetName ?? GlobalDefaultGetName;
        set => _defaultGetName = value;
    }
    #endregion

    #region DefaultGetShortName
    private static Func<EnumInfo<TEnum>, string>? _defaultGetShortName;

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetShortName
    {
        get => _defaultGetShortName ?? GlobalDefaultGetShortName;
        set => _defaultGetShortName = value;
    }
    #endregion

    #region DefaultGetDescription
    private static Func<EnumInfo<TEnum>, string>? _defaultGetDescription;

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetDescription
    {
        get => _defaultGetDescription ?? GlobalDefaultGetDescription;
        set => _defaultGetDescription = value;
    }
    #endregion

    #endregion

    #region GlobalDefault

    #region GlobalDefaultToString
    private static Lazy<Func<EnumInfo<TEnum>, string>> _globalDefaultToString =
        new(() => static v => v.Value.ToString());

    /// <summary>
    /// 全局默认到字符串方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> GlobalDefaultToString =>
        _globalDefaultToString.Value;
    #endregion

    #region GlobalDefaultGetName
    private static Lazy<Func<EnumInfo<TEnum>, string>> _globalDefaultGetName =
        new(
            () =>
                static v =>
                {
                    if (IsFlagable)
                    {
                        if (EnumDisplays.HasValue())
                        {
                            return string.Join(
                                " | ",
                                v.GetFlagInfos()
                                    .Select(static i => i.Display?.Name ?? i.Value.ToString())
                            );
                        }
                        return v.Value.ToString();
                    }
                    return v.Display?.Name ?? v.Value.ToString();
                }
        );

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> GlobalDefaultGetName => _globalDefaultGetName.Value;
    #endregion

    #region GlobalDefaultGetShortName
    private static Lazy<Func<EnumInfo<TEnum>, string>> _globalDefaultGetShortName =
        new(
            () =>
                static v =>
                {
                    if (IsFlagable)
                    {
                        if (EnumDisplays.HasValue())
                        {
                            return string.Join(
                                " | ",
                                v.GetFlagInfos()
                                    .Select(static i => i.Display?.ShortName ?? i.Value.ToString())
                            );
                        }
                        return v.Value.ToString();
                    }
                    return v.Display?.ShortName ?? v.Value.ToString();
                }
        );

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> GlobalDefaultGetShortName =>
        _globalDefaultGetShortName.Value;
    #endregion

    #region GlobalDefaultGetDescription
    private static Lazy<Func<EnumInfo<TEnum>, string>> _globalDefaultGetDescription =
        new(
            () =>
                static v =>
                {
                    if (IsFlagable)
                    {
                        if (EnumDisplays.HasValue())
                        {
                            return string.Join(
                                " | ",
                                v.GetFlagInfos()
                                    .Select(static i =>
                                        i.Display?.Description ?? i.Value.ToString()
                                    )
                            );
                        }
                        return v.Value.ToString();
                    }
                    return v.Display?.Description ?? v.Value.ToString();
                }
        );

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> GlobalDefaultGetDescription =>
        _globalDefaultGetDescription.Value;
    #endregion

    #endregion

    #endregion
}
