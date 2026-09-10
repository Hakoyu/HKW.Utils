using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
        EnumInfo.InfosByType.TryAdd(StaticEnumType, StaticInfoDictionary.Untyped);
        if (StaticInfoDictionary.TryGetValue((TEnum)@enum, out var enumInfo))
            return enumInfo;
        // 找不到意味着枚举由多个 flag 组成, 创建新枚举
        return new EnumInfo<TEnum>((TEnum)@enum, false);
    }

    /// <inheritdoc/>
    IEnumInfo IEnumInfo.Create(Enum @enum)
    {
        return Create(@enum);
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
    public DisplayAttribute? Display => EnumDisplays.GetValueOrDefault(Value, defaultValue: null);

    /// <inheritdoc/>
    public Type EnumType => EnumInfo<TEnum>.StaticEnumType;

    /// <inheritdoc/>
    public Type UnderlyingType => EnumInfo<TEnum>.StaticUnderlyingType;

    /// <inheritdoc/>
    public bool IsFlaggable => EnumInfo<TEnum>.StaticIsFlaggable;

    /// <inheritdoc/>
    public ImmutableArray<string> Names => EnumInfo<TEnum>.StaticNames;

    /// <inheritdoc/>
    public ImmutableArray<string> ValidNames => EnumInfo<TEnum>.StaticValidNames;

    #region IEnumInfoT
    /// <inheritdoc/>
    public FrozenEnumInfoDictionary<TEnum> InfoDictionary => EnumInfo<TEnum>.StaticInfoDictionary;

    /// <inheritdoc/>
    public FrozenEnumInfoDictionary<TEnum> ValidInfoDictionary =>
        EnumInfo<TEnum>.StaticValidInfoDictionary;

    /// <inheritdoc/>
    public ImmutableArray<TEnum> Values => EnumInfo<TEnum>.StaticInfoDictionary.Keys;

    /// <inheritdoc/>
    public ImmutableArray<TEnum> ValidValues => EnumInfo<TEnum>.StaticValidInfoDictionary.Keys;

    /// <inheritdoc/>
    public ImmutableArray<EnumInfo<TEnum>> Infos => EnumInfo<TEnum>.StaticInfoDictionary.Values;

    /// <inheritdoc/>
    public ImmutableArray<EnumInfo<TEnum>> ValidInfos =>
        EnumInfo<TEnum>.StaticValidInfoDictionary.Values;
    #endregion
    #region IEnumInfo
    /// <inheritdoc/>
    IDictionary<Enum, IEnumInfo> IEnumInfo.InfoDictionary =>
        EnumInfo<TEnum>.StaticInfoDictionary.Untyped;

    /// <inheritdoc/>
    IDictionary<Enum, IEnumInfo> IEnumInfo.ValidInfoDictionary =>
        EnumInfo<TEnum>.StaticValidInfoDictionary.Untyped;

    /// <inheritdoc/>
    ICollection<Enum> IEnumInfo.Values => EnumInfo<TEnum>.StaticInfoDictionary.Untyped.Keys;

    /// <inheritdoc/>
    ICollection<Enum> IEnumInfo.ValidValues =>
        EnumInfo<TEnum>.StaticValidInfoDictionary.Untyped.Keys;

    /// <inheritdoc/>
    ICollection<IEnumInfo> IEnumInfo.Infos => EnumInfo<TEnum>.StaticInfoDictionary.Untyped.Values;

    /// <inheritdoc/>
    ICollection<IEnumInfo> IEnumInfo.ValidInfos =>
        EnumInfo<TEnum>.StaticValidInfoDictionary.Untyped.Values;
    #endregion
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
        return StaticValidValues.Where(x => Value.HasFlag(x));
    }

    /// <inheritdoc/>
    public IEnumerable<IEnumInfo<TEnum>> GetFlagInfos()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return StaticValidInfoDictionary.Values.Where(x => Value.HasFlag(x));
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
        return StaticValidValues.Where(x => Value.HasFlag(x)).Cast<Enum>();
    }

    /// <inheritdoc/>
    IEnumerable<IEnumInfo> IEnumInfo.GetFlagInfos()
    {
        InvalidEnumArgumentException.ThrowIfNotFlaggable(this);
        return ((IDictionary<TEnum, EnumInfo<TEnum>>)StaticValidInfoDictionary).Values.Where(x =>
            Value.HasFlag(x.Value)
        );
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

    /// <inheritdoc cref="IEnumInfo.EnumType"/>
    public static Type StaticEnumType { get; } = typeof(TEnum);

    /// <inheritdoc cref="IEnumInfo.UnderlyingType"/>
    public static Type StaticUnderlyingType { get; } = StaticEnumType.GetEnumUnderlyingType();

    /// <inheritdoc cref="IEnumInfo.IsFlaggable"/>
    public static bool StaticIsFlaggable { get; } =
        Attribute.IsDefined(StaticEnumType, typeof(FlagsAttribute));

    ///  <inheritdoc cref="IEnumInfo.Names"/>
    public static ImmutableArray<string> StaticNames { get; } =
        Enum.GetNames<TEnum>().ToImmutableArray();

    ///  <inheritdoc cref="IEnumInfo.Values"/>
    public static ImmutableArray<TEnum> StaticValues => StaticInfoDictionary.Keys;

    ///  <inheritdoc cref="IEnumInfo.Infos"/>
    public static ImmutableArray<EnumInfo<TEnum>> StaticInfos => StaticInfoDictionary.Values;

    #region InfoDictionary
    private static readonly Lazy<FrozenEnumInfoDictionary<TEnum>> _infoDictionaryHolder = new(() =>
        new FrozenEnumInfoDictionary<TEnum>(
            Enum.GetValues<TEnum>()
                .Select(v => new KeyValuePair<TEnum, EnumInfo<TEnum>>(
                    v,
                    new EnumInfo<TEnum>(
                        v,
                        NumberUtils.CompareByF(
                            v,
                            0,
                            StaticUnderlyingType,
                            ComparisonOperatorType.Equality
                        )
                    )
                ))
        )
    );

    ///  <inheritdoc cref="IEnumInfo.InfoDictionary"/>
    public static FrozenEnumInfoDictionary<TEnum> StaticInfoDictionary =>
        _infoDictionaryHolder.Value;
    #endregion

    /// <inheritdoc cref="IEnumInfo.ValidValues"/>
    public static ImmutableArray<TEnum> StaticValidValues => StaticValidInfoDictionary.Keys;

    ///  <inheritdoc cref="IEnumInfo.Infos"/>
    public static ImmutableArray<EnumInfo<TEnum>> StaticValidInfos =>
        StaticValidInfoDictionary.Values;

    #region ValidNames
    private static readonly Lazy<ImmutableArray<string>> _validNamesHolder = new(() =>
        StaticIsFlaggable
            ? StaticValidValues.Select(x => Enum.GetName<TEnum>(x)!).ToImmutableArray()
            : StaticNames
    );

    ///  <inheritdoc cref="IEnumInfo.ValidNames"/>
    public static ImmutableArray<string> StaticValidNames => _validNamesHolder.Value;
    #endregion

    #region ValidInfos
    private static readonly Lazy<FrozenEnumInfoDictionary<TEnum>> _validInfoDictionaryHolder = new(
        () =>
            StaticIsFlaggable
                ? new(StaticInfoDictionary.Where(p => p.Value.IsNone is false))
                : StaticInfoDictionary
    );

    ///  <inheritdoc cref="IEnumInfo.ValidInfoDictionary"/>
    public static FrozenEnumInfoDictionary<TEnum> StaticValidInfoDictionary =>
        _validInfoDictionaryHolder.Value;
    #endregion
    #endregion

    /// <summary>
    /// 获取默认(首个)枚举信息
    /// </summary>
    /// <returns>信息</returns>
    public static EnumInfo<TEnum> GetInfo()
    {
        return EnumInfo<TEnum>.Create(default(TEnum));
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
            StaticValues
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
}
