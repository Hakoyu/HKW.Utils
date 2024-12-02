using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
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
    /// 清除缓存
    /// </summary>
    public static void ClearCache()
    {
        InfosByType.Clear();
    }

    /// <summary>
    /// 删除指定枚举缓存
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    public static void RemoveCache<TEnum>()
        where TEnum : struct, Enum
    {
        InfosByType.Remove(typeof(TEnum), out var _);
    }

    /// <summary>
    /// 删除指定枚举缓存
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    public static void RemoveCache(Type enumType)
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
    private static Func<IEnumInfo, string>? _globalDefaultToString;

    /// <summary>
    /// 全局默认到字符串方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultToString =>
        _globalDefaultToString ??= static v => v.Value.ToString();
    #endregion

    #region GlobalDefaultGetName
    private static Func<IEnumInfo, string>? _globalDefaultGetDisplayName;

    /// <summary>
    /// 全局默认获取名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayName =>
        _globalDefaultGetDisplayName ??= static v =>
        {
            if (v.IsFlagable is false)
                return v.Display?.Name ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.Name ?? i.Value.ToString())
            );
        };
    #endregion

    #region GlobalDefaultGetShortName
    private static Func<IEnumInfo, string>? _globalDefaultGetDisplayShortName;

    /// <summary>
    /// 全局默认获取短名称方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayShortName =>
        _globalDefaultGetDisplayShortName ??= static v =>
        {
            if (v.IsFlagable is false)
                return v.Display?.ShortName ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.ShortName ?? i.Value.ToString())
            );
        };
    #endregion

    #region GlobalDefaultGetDescription
    private static Func<IEnumInfo, string>? _globalDefaultGetDisplayDescription;

    /// <summary>
    /// 全局默认获取描述方法
    /// </summary>
    public static Func<IEnumInfo, string> GlobalDefaultGetDisplayDescription =>
        _globalDefaultGetDisplayDescription ??= static v =>
        {
            if (v.IsFlagable is false)
                return v.Display?.Description ?? v.Value.ToString();
            return string.Join(
                ", ",
                v.GetFlagInfos().Select(static i => i.Display?.Description ?? i.Value.ToString())
            );
        };
    #endregion

    #endregion

    /// <summary>
    /// 创建枚举信息表达式
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <returns>构造方法</returns>
    internal static Func<Enum, IEnumInfo> CreateEnumInfoExpression(Type enumType)
    {
        var enumParam = Expression.Parameter(typeof(Enum), "enumValue");
        var createMethod = typeof(EnumInfo<>)
            .MakeGenericType(enumType)
            .GetMethod(nameof(EnumInfo<StringComparison>.Create), new[] { typeof(Enum) });
        var createCall = Expression.Call(createMethod!, enumParam);
        var lambda = Expression.Lambda<Func<Enum, IEnumInfo>>(createCall, enumParam);
        return lambda.Compile();
    }
}
