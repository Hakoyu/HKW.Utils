using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace HKW.HKWUtils;

/// <summary>
/// 枚举信息接口
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public interface IEnumInfo<TEnum> : IEnumInfo, IEquatable<IEnumInfo<TEnum>>, IEquatable<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public new TEnum Value { get; }

    /// <inheritdoc cref="IEnumInfo.InfoDictionary"/>
    public new FrozenEnumInfoDictionary<TEnum> InfoDictionary { get; }

    /// <inheritdoc cref="IEnumInfo.ValidInfoDictionary"/>
    public new FrozenEnumInfoDictionary<TEnum> ValidInfoDictionary { get; }

    /// <inheritdoc cref="IEnumInfo.Values"/>
    public new ImmutableArray<TEnum> Values { get; }

    /// <inheritdoc cref="IEnumInfo.ValidValues"/>
    public new ImmutableArray<TEnum> ValidValues { get; }

    /// <inheritdoc cref="IEnumInfo.Infos"/>
    public new ImmutableArray<EnumInfo<TEnum>> Infos { get; }

    /// <inheritdoc cref="IEnumInfo.ValidInfos"/>
    public new ImmutableArray<EnumInfo<TEnum>> ValidInfos { get; }

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>是否有标记</returns>
    public bool HasFlag(TEnum flag);

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>是否有标记</returns>
    public bool HasFlag(IEnumInfo<TEnum> flag);

    /// <summary>
    /// 获取标志
    /// </summary>
    /// <returns>全部标志</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public new IEnumerable<TEnum> GetFlags();

    /// <summary>
    /// 获取标志信息
    /// </summary>
    /// <returns>全部标志信息</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public new IEnumerable<IEnumInfo<TEnum>> GetFlagInfos();
}

/// <summary>
/// 枚举信息接口
/// </summary>
public interface IEnumInfo
{
    /// <summary>
    /// 枚举值
    /// </summary>
    public Enum Value { get; }

    /// <summary>
    /// 枚举是否为 0 占位符
    /// </summary>
    public bool IsNone { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// 短名称
    /// </summary>
    public string DisplayShortName { get; }

    /// <summary>
    /// 描述
    /// </summary>
    public string DisplayDescription { get; }

    /// <summary>
    /// 显示
    /// </summary>
    public DisplayAttribute? Display { get; }

    /// <summary>
    /// 枚举类型
    /// </summary>
    public Type EnumType { get; }

    /// <summary>
    /// 基础类型
    /// </summary>
    public Type UnderlyingType { get; }

    /// <summary>
    /// 是可标记的
    /// </summary>
    public bool IsFlaggable { get; }

    /// <summary>
    /// 全部名称
    /// </summary>
    public ImmutableArray<string> Names { get; }

    /// <summary>
    /// 有效的全部名称 (为设置 <see cref="FlagsAttribute"/> 的枚举排除None)
    /// </summary>
    public ImmutableArray<string> ValidNames { get; }

    /// <summary>
    /// 信息字典
    /// </summary>
    public IDictionary<Enum, IEnumInfo> InfoDictionary { get; }

    /// <summary>
    /// 全部有效字典 (为设置 <see cref="FlagsAttribute"/> 的枚举排除None)
    /// </summary>
    public IDictionary<Enum, IEnumInfo> ValidInfoDictionary { get; }

    /// <summary>
    /// 全部值
    /// </summary>
    public ICollection<Enum> Values { get; }

    /// <summary>
    /// 全部有效值
    /// </summary>
    public ICollection<Enum> ValidValues { get; }

    /// <summary>
    /// 全部信息
    /// </summary>
    public ICollection<IEnumInfo> Infos { get; }

    /// <summary>
    /// 全部有效信息
    /// </summary>
    public ICollection<IEnumInfo> ValidInfos { get; }

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>是否有标记</returns>
    public bool HasFlag(Enum flag);

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>是否有标记</returns>
    public bool HasFlag(IEnumInfo flag);

    /// <summary>
    /// 获取标志
    /// </summary>
    /// <returns>全部标志</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public IEnumerable<Enum> GetFlags();

    /// <summary>
    /// 获取标志信息
    /// </summary>
    /// <returns>全部标志信息</returns>
    /// <exception cref="Exception">枚举没有特性 <see cref="FlagsAttribute"/></exception>
    public IEnumerable<IEnumInfo> GetFlagInfos();

    /// <summary>
    /// 创建一个新的枚举信息
    /// </summary>
    /// <returns>枚举信息</returns>
    public IEnumInfo Create(Enum @enum);
}
