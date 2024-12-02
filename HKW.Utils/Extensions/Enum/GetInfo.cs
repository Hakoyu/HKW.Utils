using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    /// <returns>枚举信息</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumInfo<TEnum> GetInfo<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        return EnumInfo<TEnum>.GetInfo(value);
    }

    /// <summary>
    /// 获取枚举信息
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>枚举信息</returns>
    public static IEnumInfo GetInfo(this Enum value)
    {
        var type = value.GetType();
        if (EnumInfo.InfosByType.TryGetValue(type, out var infos))
            return infos.Values.First().Create(value);
        return EnumInfo.CreateEnumInfoExpression(type).Invoke(value);
    }

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">枚举值</param>
    /// <param name="target">目标</param>
    /// <returns>目标信息</returns>
    public static string GetDisplayInfo<TEnum>(this TEnum value, EnumInfoDisplayTarget target)
        where TEnum : struct, Enum
    {
        var info = value.GetInfo();
        return target switch
        {
            EnumInfoDisplayTarget.Name => info.DisplayName,
            EnumInfoDisplayTarget.ShortName => info.DisplayShortName,
            EnumInfoDisplayTarget.Description => info.DisplayDescription,
            _ => info.DisplayName,
        };
    }

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="target">目标</param>
    /// <returns>目标信息</returns>
    public static string GetDisplayInfo(this Enum value, EnumInfoDisplayTarget target)
    {
        var info = value.GetInfo();
        return target switch
        {
            EnumInfoDisplayTarget.Name => info.DisplayName,
            EnumInfoDisplayTarget.ShortName => info.DisplayShortName,
            EnumInfoDisplayTarget.Description => info.DisplayDescription,
            _ => info.DisplayName,
        };
    }
}
