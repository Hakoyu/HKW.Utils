using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils;

/// <summary>
/// 枚举信息
/// </summary>
public static class EnumInfo
{
    /// <summary>
    /// (EnumType, (EnumValue, EnumInfo))
    /// </summary>
    internal static ConcurrentDictionary<
        Type,
        FrozenDictionary<Enum, IEnumInfo>
    > InfosByType { get; } = [];

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="enum">枚举值</param>
    /// <returns>枚举信息接口</returns>
    public static IEnumInfo? GetEnumInfo(Type type, Enum @enum)
    {
        if (TryGetEnumInfo(type, @enum, out var info) is false)
            throw new InvalidOperationException(
                "The specified enumeration type is not initialized, use EnumInfo<Enum>.Initialize() first."
            );
        return info;
    }

    /// <summary>
    /// 尝试获取枚举信息
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="enum">枚举值</param>
    /// <param name="info">枚举信息</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetEnumInfo(
        Type type,
        Enum @enum,
        [MaybeNullWhen(false)] out IEnumInfo info
    )
    {
        info = default;
        if (InfosByType.TryGetValue(type, out var infos) is false)
            return false;
        if (infos.TryGetValue(@enum, out info) is false)
            return false;
        return true;
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static EnumInfo<TEnum> GetEnumInfo<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        EnumInfo<TEnum>.Initialize();
        return (EnumInfo<TEnum>)InfosByType[typeof(TEnum)][@enum];
    }

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="info">枚举信息</param>
    /// <param name="target">目标</param>
    /// <returns>目标信息</returns>
    public static string GetInfo(this IEnumInfo info, EnumInfoTarget target)
    {
        return target switch
        {
            EnumInfoTarget.Name => info.Name,
            EnumInfoTarget.ShortName => info.ShortName,
            EnumInfoTarget.Description => info.Description,
            _ => info.Name,
        };
    }

    #region Default

    #region DefaultToString

    private static Func<IEnumInfo, string>? _defaultToString;

    /// <summary>
    /// 默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultToString
    {
        get => _defaultToString ?? GlobalDefaultToString;
        set => _defaultToString = value;
    }
    #endregion

    #region DefaultGetName
    private static Func<IEnumInfo, string>? _defaultGetName;

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetName
    {
        get => _defaultGetName ?? GlobalDefaultGetName;
        set => _defaultGetName = value;
    }
    #endregion

    #region DefaultGetShortName
    private static Func<IEnumInfo, string>? _defaultGetShortName;

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetShortName
    {
        get => _defaultGetShortName ?? GlobalDefaultGetShortName;
        set => _defaultGetShortName = value;
    }
    #endregion

    #region DefaultGetDescription
    private static Func<IEnumInfo, string>? _defaultGetDescription;

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDescription
    {
        get => _defaultGetDescription ?? GlobalDefaultGetDescription;
        set => _defaultGetDescription = value;
    }
    #endregion

    #endregion



    #region GlobalDefault

    #region GlobalDefaultToString
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultToString =
        new(() => static v => v.Value.ToString());

    /// <summary>
    /// 全局默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultToString => _globalDefaultToString.Value;
    #endregion

    #region GlobalDefaultGetName
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetName =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.Name ?? v.Value.ToString();
                    return string.Join(
                        " | ",
                        v.GetFlagInfos().Select(static i => i.Display?.Name ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetName => _globalDefaultGetName.Value;
    #endregion

    #region GlobalDefaultGetShortName
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetShortName =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.ShortName ?? v.Value.ToString();
                    return string.Join(
                        " | ",
                        v.GetFlagInfos()
                            .Select(static i => i.Display?.ShortName ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetShortName =>
        _globalDefaultGetShortName.Value;
    #endregion

    #region GlobalDefaultGetDescription
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetDescription =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.Description ?? v.Value.ToString();
                    return string.Join(
                        " | ",
                        v.GetFlagInfos()
                            .Select(static i => i.Display?.Description ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDescription =>
        _globalDefaultGetDescription.Value;
    #endregion

    #endregion
}
