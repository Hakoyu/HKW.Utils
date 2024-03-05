using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 删除标签
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="enum">枚举</param>
    /// <param name="flag">标签</param>
    /// <returns>删除标签的枚举</returns>
    public static TEnum RemoveFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(@enum) & ~Convert.ToInt64(flag));
    }
}
