using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

/// <summary>
/// 枚举信息
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
[DebuggerDisplay("{Value}")]
public sealed class EnumInfo<TEnum> : IEnumInfo<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    private EnumInfo(TEnum value, bool isNone)
    {
        Value = value;
        IsNone = isNone;
    }

    /// <inheritdoc/>
    public static EnumInfo<TEnum> Create(Enum @enum)
    {
        ArgumentNullException.ThrowIfNull(@enum);
        EnumInfo.InfosByType.TryAdd(StaticEnumType, StaticInfos);
        if (StaticInfos.TryGetValue(@enum, out var enumInfo))
            return (EnumInfo<TEnum>)enumInfo;
        // 找不到意味着枚举由多个 flag 组成, 创建新枚举
        return new EnumInfo<TEnum>((TEnum)@enum, false);
    }

    /// <inheritdoc/>
    IEnumInfo IEnumInfo.Create(Enum @enum)
    {
        ArgumentNullException.ThrowIfNull(@enum);
        EnumInfo.InfosByType.TryAdd(StaticEnumType, StaticInfos);
        if (StaticInfos.TryGetValue(@enum, out var enumInfo))
            return enumInfo;
        // 找不到意味着枚举由多个 flag 组成, 创建新枚举
        return new EnumInfo<TEnum>((TEnum)@enum, false);
    }

    /// <inheritdoc/>
    public TEnum Value { get; }
    Enum IEnumInfo.Value => Value;

    /// <inheritdoc/>
    public bool IsNone { get; }

    /// <inheritdoc/>
    public string DisplayName => GetDisplayName(this);

    /// <inheritdoc/>
    public string DisplayShortName => GetDisplayShortName(this);

    /// <inheritdoc/>
    public string DisplayDescription => GetDisplayDescription(this);

    /// <inheritdoc/>
    public DisplayAttribute? Display => EnumDisplays!.GetValueOrDefault(Value, defaultValue: null);

    /// <inheritdoc/>
    public Type EnumType => EnumInfo<TEnum>.StaticEnumType;

    /// <inheritdoc/>
    public Type UnderlyingType => EnumInfo<TEnum>.StaticUnderlyingType;

    /// <inheritdoc/>
    public bool IsFlaggable => EnumInfo<TEnum>.StaticIsFlaggable;

    /// <inheritdoc/>
    public FrozenSet<string> Names => EnumInfo<TEnum>.StaticNames;

    /// <inheritdoc/>
    public FrozenDictionary<Enum, IEnumInfo> Infos => EnumInfo<TEnum>.StaticInfos;

    /// <inheritdoc/>
    public FrozenSet<string> ValidNames => EnumInfo<TEnum>.StaticValidNames;

    /// <inheritdoc/>
    public FrozenDictionary<Enum, IEnumInfo> ValidInfos => EnumInfo<TEnum>.StaticValidInfos;

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
    public IEnumerable<TEnum> GetFlags()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return ValidValues.Where(x => Value.HasFlag(x));
    }

    /// <inheritdoc/>
    public IEnumerable<IEnumInfo<TEnum>> GetFlagInfos()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return StaticValidInfos.Values.Cast<EnumInfo<TEnum>>().Where(x => Value.HasFlag(x));
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
    IEnumerable<Enum> IEnumInfo.GetFlags()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return ValidValues.Where(x => Value.HasFlag(x)).Cast<Enum>();
    }

    /// <inheritdoc/>
    IEnumerable<IEnumInfo> IEnumInfo.GetFlagInfos()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return StaticValidInfos.Values.Where(x => Value.HasFlag(x.Value));
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
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public bool Equals(TEnum other)
    {
        return Value == other;
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
    #endregion

    #region static

    /// <summary>
    /// 枚举类型
    /// </summary>
    public static Type StaticEnumType { get; } = typeof(TEnum);

    /// <summary>
    /// 基础类型
    /// </summary>
    public static Type StaticUnderlyingType { get; } = StaticEnumType.GetEnumUnderlyingType();

    /// <summary>
    /// 是可标记的
    /// </summary>
    public static bool StaticIsFlaggable { get; } =
        Attribute.IsDefined(StaticEnumType, typeof(FlagsAttribute));

    #region Names
    private static readonly Lazy<FrozenSet<string>> _namesHolder = new(() =>
        Enum.GetNames<TEnum>().ToFrozenSet()
    );

    /// <summary>
    /// 全部名称
    /// </summary>
    public static FrozenSet<string> StaticNames => _namesHolder.Value;
    #endregion

    #region Values
    private static readonly Lazy<FrozenSet<TEnum>> _valuesHolder = new(() =>
        Enum.GetValues<TEnum>().ToFrozenSet()
    );

    /// <summary>
    /// 全部值
    /// </summary>
    public static FrozenSet<TEnum> Values => _valuesHolder.Value;
    #endregion

    #region Infos
    private static readonly Lazy<FrozenDictionary<Enum, IEnumInfo>> _infosHolder = new(() =>
        Values.ToFrozenDictionary(
            v => (Enum)v,
            v =>
                (IEnumInfo)
                    new EnumInfo<TEnum>(
                        v,
                        NumberUtils.CompareByF(
                            v,
                            0,
                            StaticUnderlyingType,
                            ComparisonOperatorType.Equality
                        )
                    )
        )
    );

    /// <summary>
    /// 全部信息
    /// </summary>
    public static FrozenDictionary<Enum, IEnumInfo> StaticInfos => _infosHolder.Value;
    #endregion
    #region ValidEnum

    #region ValidValues
    private static readonly Lazy<FrozenSet<TEnum>> _validValuesHolder = new(() =>
        StaticIsFlaggable
            ? StaticInfos
                .Where(p => p.Value.IsNone is false)
                .Select(p => (TEnum)p.Key)
                .ToFrozenSet()
            : Values
    );

    /// <summary>
    /// 有效的全部值 (排除None)
    /// </summary>
    public static FrozenSet<TEnum> ValidValues => _validValuesHolder.Value;
    #endregion

    #region ValidNames
    private static readonly Lazy<FrozenSet<string>> _validNamesHolder = new(() =>
        StaticIsFlaggable
            ? ValidValues.Select(x => Enum.GetName<TEnum>(x)).ToFrozenSet()!
            : StaticNames
    );

    /// <summary>
    /// 有效的全部名称 (排除None)
    /// </summary>
    public static FrozenSet<string> StaticValidNames => _validNamesHolder.Value;
    #endregion

    #region ValidInfos
    private static readonly Lazy<FrozenDictionary<Enum, IEnumInfo>> _validInfosHolder = new(() =>
        StaticIsFlaggable
            ? StaticInfos.Where(p => p.Value.IsNone is false).ToFrozenDictionary()
            : StaticInfos
    );

    /// <summary>
    /// 有效的全部信息 (排除None)
    /// </summary>
    public static FrozenDictionary<Enum, IEnumInfo> StaticValidInfos => _validInfosHolder.Value;
    #endregion
    #endregion

    /// <summary>
    /// 获取默认(首个)枚举信息
    /// </summary>
    /// <returns>信息</returns>
    public static EnumInfo<TEnum> GetInfo()
    {
        return EnumInfo<TEnum>.Create(default!);
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <param name="enum">枚举值</param>
    /// <returns>信息</returns>
    public static EnumInfo<TEnum> GetInfo(TEnum @enum)
    {
        return EnumInfo<TEnum>.Create(@enum);
    }

    #region EnumDisplays
    private static readonly Lazy<FrozenDictionary<TEnum, DisplayAttribute?>> _enumDisplaysHolder =
        new(() =>
            Values
                .Select(static v => (Value: v, FieldInfo: StaticEnumType.GetField(v.ToString())!))
                .ToFrozenDictionary(
                    v => v.Value,
                    v => v.FieldInfo.GetCustomAttribute<DisplayAttribute>()
                )
        );

    /// <summary>
    /// 枚举信息
    /// <para>
    /// (Enum, DisplayAttribute)
    /// </para>
    /// </summary>
    public static FrozenDictionary<TEnum, DisplayAttribute?> EnumDisplays =>
        _enumDisplaysHolder.Value;

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
