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
                $"The specified enum type [{type.FullName}] is not initialized, use EnumInfo<Enum>.Initialize() first."
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
            info = infos.First().Value.Create(@enum);
        return true;
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <returns>枚举信息接口</returns>
    public static IEnumInfo? GetEnumInfo(Type type)
    {
        if (TryGetEnumInfo(type, out var info) is false)
            throw new InvalidOperationException(
                $"The specified enum type [{type.FullName}] is not initialized, use EnumInfo<Enum>.Initialize() first."
            );
        return info;
    }

    /// <summary>
    /// 尝试获取枚举信息
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="info">枚举信息</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetEnumInfo(Type type, [MaybeNullWhen(false)] out IEnumInfo info)
    {
        info = default;
        if (InfosByType.TryGetValue(type, out var infos) is false)
            return false;
        info = infos.First().Value;
        return true;
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>枚举信息</returns>
    public static EnumInfo<TEnum> GetEnumInfo<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        EnumInfo<TEnum>.Initialize();
        var infos = InfosByType[typeof(TEnum)];
        if (infos.TryGetValue(@enum, out var info) is false)
            info = infos.First().Value.Create(@enum);
        return (EnumInfo<TEnum>)info;
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <returns>枚举信息</returns>
    public static EnumInfo<TEnum> GetEnumInfo<TEnum>()
        where TEnum : struct, Enum
    {
        EnumInfo<TEnum>.Initialize();
        return (EnumInfo<TEnum>)InfosByType[typeof(TEnum)].First().Value;
    }

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="info">枚举信息</param>
    /// <param name="target">目标</param>
    /// <returns>目标信息</returns>
    public static string GetInfo(this IEnumInfo info, EnumInfoDisplayTarget target)
    {
        return target switch
        {
            EnumInfoDisplayTarget.Name => info.DisplayName,
            EnumInfoDisplayTarget.ShortName => info.DisplayShortName,
            EnumInfoDisplayTarget.Description => info.DisplayDescription,
            _ => info.DisplayName,
        };
    }

    /// <summary>
    /// 清除缓存
    /// </summary>
    public static void ClearCache()
    {
        InfosByType.Clear();
    }

    /// <summary>
    /// 清除指定枚举缓存
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    public static void ClearCache<TEnum>()
        where TEnum : struct, Enum
    {
        InfosByType.Remove(typeof(TEnum), out var _);
    }

    /// <summary>
    /// 清除指定枚举缓存
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    public static void ClearCache(Type enumType)
    {
        InfosByType.Remove(enumType, out var _);
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
    private static Func<IEnumInfo, string>? _defaultGetDisplayName;

    /// <summary>
    /// 默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayName
    {
        get => _defaultGetDisplayName ?? GlobalDefaultGetDisplayName;
        set => _defaultGetDisplayName = value;
    }
    #endregion

    #region DefaultGetShortName
    private static Func<IEnumInfo, string>? _defaultGetDisplayShortName;

    /// <summary>
    /// 默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayShortName
    {
        get => _defaultGetDisplayShortName ?? GlobalDefaultGetDisplayShortName;
        set => _defaultGetDisplayShortName = value;
    }
    #endregion

    #region DefaultGetDescription
    private static Func<IEnumInfo, string>? _defaultGetDisplayDescription;

    /// <summary>
    /// 默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> DefaultGetDisplayDescription
    {
        get => _defaultGetDisplayDescription ?? GlobalDefaultGetDisplayDescription;
        set => _defaultGetDisplayDescription = value;
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
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetDisplayName =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.Name ?? v.Value.ToString();
                    return string.Join(
                        ", ",
                        v.GetFlagInfos().Select(static i => i.Display?.Name ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayName =>
        _globalDefaultGetDisplayName.Value;
    #endregion

    #region GlobalDefaultGetShortName
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetDisplayShortName =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.ShortName ?? v.Value.ToString();
                    return string.Join(
                        ", ",
                        v.GetFlagInfos()
                            .Select(static i => i.Display?.ShortName ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayShortName =>
        _globalDefaultGetDisplayShortName.Value;
    #endregion

    #region GlobalDefaultGetDescription
    private static Lazy<Func<IEnumInfo, string>> _globalDefaultGetDisplayDescription =
        new(
            () =>
                static v =>
                {
                    if (v.IsFlagable is false)
                        return v.Display?.Description ?? v.Value.ToString();
                    return string.Join(
                        ", ",
                        v.GetFlagInfos()
                            .Select(static i => i.Display?.Description ?? i.Value.ToString())
                    );
                }
        );

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayDescription =>
        _globalDefaultGetDisplayDescription.Value;
    #endregion

    #endregion
}
