using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
    internal static readonly Dictionary<Type, FrozenDictionary<Enum, IEnumInfo>> InfosByType = [];

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <param name="enum">枚举值</param>
    /// <returns>枚举信息接口</returns>
    public static IEnumInfo? GetEnumInfo(Type type, Enum @enum)
    {
        if (InfosByType.TryGetValue(type, out var infos) is false)
            throw new InvalidOperationException(
                "The specified enumeration type is not initialized, use EnumInfo<Enum>.Initialize() first."
            );
        return infos[@enum];
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
