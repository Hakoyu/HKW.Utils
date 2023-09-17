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
    /// 尝试获取枚举项的 <see cref="IList{T}"/> 接口
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="enumerable">枚举项</param>
    /// <param name="iList">获取的接口</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    /// <exception cref="Exception"></exception>
    public static bool GetIList<T>(
        this IEnumerable<T> enumerable,
        [NotNullWhen(true)] out IList<T>? iList
    )
    {
        if (enumerable is IList<T> list)
        {
            iList = list;
            return true;
        }
        else
        {
            iList = null;
            return false;
        }
    }
}
