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
/// 枚举信息
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
[DebuggerDisplay("{Value}")]
public class EnumInfo<TEnum> : IEnumInfo<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public EnumInfo(TEnum value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public static EnumInfo<TEnum> Create(Enum @enum)
    {
        EnumInfo.InfosByType.TryAdd(EnumType, Infos);
        return new EnumInfo<TEnum>((TEnum)@enum);
    }

    /// <inheritdoc/>
    IEnumInfo IEnumInfo.Create(Enum @enum)
    {
        return new EnumInfo<TEnum>((TEnum)@enum);
    }

    /// <inheritdoc/>
    public TEnum Value { get; }
    Enum IEnumInfo.Value => Value;

    /// <inheritdoc/>
    public string DisplayName => GetDisplayName(this);

    /// <inheritdoc/>
    public string DisplayShortName => GetDisplayShortName(this);

    /// <inheritdoc/>
    public string DisplayDescription => GetDisplayDescription(this);

    /// <inheritdoc/>
    public DisplayAttribute? Display =>
        EnumDisplays is null ? null : EnumDisplays!.GetValueOrDefault(Value, defaultValue: null);

    Type IEnumInfo.EnumType => EnumInfo<TEnum>.EnumType;

    Type IEnumInfo.UnderlyingType => EnumInfo<TEnum>.UnderlyingType;

    bool IEnumInfo.IsFlagable => EnumInfo<TEnum>.IsFlagable;

    FrozenSet<string> IEnumInfo.Names => EnumInfo<TEnum>.Names;

    FrozenDictionary<Enum, IEnumInfo> IEnumInfo.Infos => EnumInfo<TEnum>.Infos;

    FrozenSet<string> IEnumInfo.ValidNames => EnumInfo<TEnum>.ValidNames;

    FrozenDictionary<Enum, IEnumInfo> IEnumInfo.ValidInfos => EnumInfo<TEnum>.ValidInfos;

    #region GetName
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<EnumInfo<TEnum>, string>? _getDisplayName;

    /// <summary>
    /// 获取名称
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetDisplayName
    {
        get => _getDisplayName ?? DefaultGetDisplayName;
        set => _getDisplayName = value;
    }
    #endregion

    #region GetShortName
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<EnumInfo<TEnum>, string>? _getDisplayShortName;

    /// <summary>
    /// 获取短名称
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetDisplayShortName
    {
        get => _getDisplayShortName ?? DefaultGetDisplayShortName;
        set => _getDisplayShortName = value;
    }
    #endregion

    #region GetDescription
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<EnumInfo<TEnum>, string>? _getDisplayDescription;

    /// <summary>
    /// 获取描述
    /// </summary>
    public Func<EnumInfo<TEnum>, string> GetDisplayDescription
    {
        get => _getDisplayDescription ?? DefaultGetDisplayDescription;
        set => _getDisplayDescription = value;
    }
    #endregion

    #region ToString
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<EnumInfo<TEnum>, string>? _toStringFunc;

    /// <summary>
    /// 到字符串方法
    /// </summary>
    public Func<EnumInfo<TEnum>, string> ToStringFunc
    {
        get => _toStringFunc ?? DefaultToString;
        set => _toStringFunc = value;
    }
    #endregion

    #region IEnumInfo
    /// <inheritdoc/>
    bool IEnumInfo.HasFlag(Enum flag)
    {
        return Value.HasFlag(flag);
    }

    /// <inheritdoc/>
    bool IEnumInfo.HasFlag(IEnumInfo flag)
    {
        return Value.HasFlag(flag.Value);
    }

    /// <inheritdoc/>
    IEnumerable<Enum> IEnumInfo.GetFlags(bool onlyValid)
    {
        if (IsFlagable is false)
            throw new Exception($"Enum \"{EnumType}\" not use \"{nameof(FlagsAttribute)}\".");
        if (onlyValid)
            return ValidValues.Where(i => Value.HasFlag(i)).Cast<Enum>();
        return Values.Where(i => Value.HasFlag(i)).Cast<Enum>();
    }

    /// <inheritdoc/>
    IEnumerable<IEnumInfo> IEnumInfo.GetFlagInfos(bool onlyValid)
    {
        if (IsFlagable is false)
            throw new Exception($"Enum \"{EnumType}\" not use \"{nameof(FlagsAttribute)}\".");
        if (onlyValid)
            return ValidInfos.Values.Where(i => Value.HasFlag(i.Value));
        return Infos.Values.Where(i => Value.HasFlag(i.Value));
    }
    #endregion

    #region IEnumInfoT
    /// <inheritdoc/>
    public bool HasFlag(TEnum flag)
    {
        return Value.HasFlag(flag);
    }

    /// <inheritdoc/>
    public bool HasFlag(IEnumInfo<TEnum> flag)
    {
        return Value.HasFlag(flag.Value);
    }

    /// <inheritdoc/>
    public IEnumerable<TEnum> GetFlags(bool onlyValid = true)
    {
        if (IsFlagable is false)
            throw new Exception($"Enum \"{EnumType}\" not use \"{nameof(FlagsAttribute)}\".");
        if (onlyValid)
            return ValidValues.Where(i => Value.HasFlag(i));
        return Values.Where(i => Value.HasFlag(i));
    }

    /// <inheritdoc/>
    public IEnumerable<IEnumInfo<TEnum>> GetFlagInfos(bool onlyValid = true)
    {
        if (IsFlagable is false)
            throw new Exception($"Enum \"{EnumType}\" not use \"{nameof(FlagsAttribute)}\".");
        if (onlyValid)
            return ValidInfos.Values.Cast<EnumInfo<TEnum>>().Where(i => Value.HasFlag(i));
        return Infos.Values.Cast<EnumInfo<TEnum>>().Where(i => Value.HasFlag(i));
    }

    #endregion
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
    public bool Equals(TEnum other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IEnumInfo<TEnum>);
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
    /// 枚举类型
    /// </summary>
    public static Type EnumType { get; } = typeof(TEnum);

    /// <summary>
    /// 基础类型
    /// </summary>
    public static Type UnderlyingType { get; } = EnumType.GetEnumUnderlyingType();

    /// <summary>
    /// 是可标记的
    /// </summary>
    public static bool IsFlagable { get; } = Attribute.IsDefined(EnumType, typeof(FlagsAttribute));

    /// <summary>
    /// 全部名称
    /// </summary>
    public static FrozenSet<string> Names { get; } = Enum.GetNames<TEnum>().ToFrozenSet();

    /// <summary>
    /// 全部值
    /// </summary>
    public static FrozenSet<TEnum> Values { get; } = Enum.GetValues<TEnum>().ToFrozenSet();

    /// <summary>
    /// 全部信息
    /// </summary>
    public static FrozenDictionary<Enum, IEnumInfo> Infos { get; } =
        Values.ToFrozenDictionary(v => (Enum)v, v => (IEnumInfo)new EnumInfo<TEnum>(v));

    #region ValidEnum

    /// <summary>
    /// 有效的全部值 (去除0值)
    /// </summary>
    public static FrozenSet<TEnum> ValidValues { get; } =
        Enum.GetValues<TEnum>()
            .Where(x =>
                NumberUtils.CompareX(x, 0, UnderlyingType, ComparisonOperatorType.Inequality)
            )
            .ToFrozenSet();

    /// <summary>
    /// 有效的全部名称 (去除0值)
    /// </summary>
    public static FrozenSet<string> ValidNames { get; } =
        ValidValues.Select(x => Enum.GetName<TEnum>(x)).ToFrozenSet()!;

    /// <summary>
    /// 有效的全部信息 (去除0值)
    /// </summary>
    public static FrozenDictionary<Enum, IEnumInfo> ValidInfos { get; } =
        ValidValues.ToFrozenDictionary(v => (Enum)v, v => (IEnumInfo)new EnumInfo<TEnum>(v));
    #endregion

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="enum">枚举值</param>
    /// <returns>信息</returns>
    public static EnumInfo<TEnum> GetInfo(TEnum @enum)
    {
        if (Infos.TryGetValue(@enum, out var info))
            return (EnumInfo<TEnum>)info;
        return (EnumInfo<TEnum>)Infos.First().Value.Create(@enum);
    }

    #region EnumDisplays
    private static FrozenDictionary<TEnum, DisplayAttribute>? _enumDisplays;

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (Enum, DisplayAttribute)
    /// </para>
    /// </summary>
    public static FrozenDictionary<TEnum, DisplayAttribute> EnumDisplays =>
        _enumDisplays ??= Values
            .Select(static v => (Value: v, FieldInfo: EnumType.GetField(v.ToString())!))
            .Where(static v => v.FieldInfo.IsDefined(typeof(DisplayAttribute)))
            .ToFrozenDictionary(
                v => v.Value,
                v => v.FieldInfo.GetCustomAttribute<DisplayAttribute>()!
            );

    #endregion

    #region Default

    #region DefaultToString
    private static Func<EnumInfo<TEnum>, string>? _defaultToString;

    /// <summary>
    /// 默认到字符串方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultToString
    {
        get => _defaultToString ?? EnumInfo.DefaultToString;
        set => _defaultToString = value;
    }
    #endregion

    #region DefaultGetName
    private static Func<EnumInfo<TEnum>, string>? _defaultGetDisplayName;

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetDisplayName
    {
        get => _defaultGetDisplayName ?? EnumInfo.DefaultGetDisplayName;
        set => _defaultGetDisplayName = value;
    }
    #endregion

    #region DefaultGetShortName
    private static Func<EnumInfo<TEnum>, string>? _defaultGetDisplayShortName;

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetDisplayShortName
    {
        get => _defaultGetDisplayShortName ?? EnumInfo.DefaultGetDisplayShortName;
        set => _defaultGetDisplayShortName = value;
    }
    #endregion

    #region DefaultGetDescription
    private static Func<EnumInfo<TEnum>, string>? _defaultGetDisplayDescription;

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<EnumInfo<TEnum>, string> DefaultGetDisplayDescription
    {
        get => _defaultGetDisplayDescription ?? EnumInfo.DefaultGetDisplayDescription;
        set => _defaultGetDisplayDescription = value;
    }
    #endregion

    #endregion

    #endregion
}
