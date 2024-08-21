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
    /// 添加标签
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">值</param>
    /// <param name="flag">标签</param>
    /// <returns>添加标签的枚举</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum AddFlag<TEnum>(this TEnum value, TEnum flag)
        where TEnum : struct, Enum
    {
        Enum.GetUnderlyingType(typeof(TEnum));
        return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) | Convert.ToInt64(flag));
    }
}
