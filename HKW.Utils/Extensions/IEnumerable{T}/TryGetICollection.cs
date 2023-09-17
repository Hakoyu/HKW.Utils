using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 尝试获取枚举项的 <see cref="ICollection{T}"/> 接口
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="enumerable">枚举项</param>
    /// <param name="iCollection">获取的接口</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    /// <exception cref="Exception"></exception>
    public static bool TryGetICollection<T>(
        this IEnumerable<T> enumerable,
        [NotNullWhen(true)] out ICollection<T>? iCollection
    )
    {
        if (enumerable is ICollection<T> collection)
        {
            iCollection = collection;
            return true;
        }
        else
        {
            iCollection = null;
            return false;
        }
    }
}
