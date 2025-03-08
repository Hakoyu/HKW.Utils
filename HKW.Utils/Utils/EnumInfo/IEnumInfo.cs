using System.Collections.Frozen;
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

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
    public bool HasFlag(TEnum flag);

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
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
    public bool IsFlagable { get; }

    /// <summary>
    /// 全部名称
    /// </summary>
    public FrozenSet<string> Names { get; }

    /// <summary>
    /// 全部信息
    /// </summary>
    public FrozenDictionary<Enum, IEnumInfo> Infos { get; }

    /// <summary>
    /// 有效的全部名称 (为设置 <see cref="FlagsAttribute"/> 的枚举排除None)
    /// </summary>
    public FrozenSet<string> ValidNames { get; }

    /// <summary>
    /// 有效的全部信息 (为设置 <see cref="FlagsAttribute"/> 的枚举排除None)
    /// </summary>
    public FrozenDictionary<Enum, IEnumInfo> ValidInfos { get; }

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
    public bool HasFlag(Enum flag);

    /// <summary>
    /// 拥有标记
    /// </summary>
    /// <param name="flag">标记</param>
    /// <returns>有标记为 <see langword="true"/> 没有为 <see langword="false"/></returns>
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
