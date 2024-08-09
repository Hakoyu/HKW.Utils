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
}
