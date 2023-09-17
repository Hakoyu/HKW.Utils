using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取枚举项的 <see cref="ICollection{T}"/> 接口
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="enumerable">枚举项</param>
    /// <param name="createWhenFalse">若 <paramref name="enumerable"/> 不包含 <see cref="ICollection{T}"/> 接口, 则创建一个新的</param>
    /// <returns>获取的接口</returns>
    /// <exception cref="Exception"></exception>
    public static ICollection<T> GetICollection<T>(
        this IEnumerable<T> enumerable,
        bool createWhenFalse = false
    )
    {
        if (enumerable is ICollection<T> list)
            return list;
        else if (createWhenFalse)
            return enumerable.ToList();
        else
            throw new Exception("Failed to get");
    }
}
