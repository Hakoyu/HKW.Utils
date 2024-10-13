using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 包含索引值
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">索引</param>
    /// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsIndex<T>(this ICollection<T> list, int index)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        if (index >= 0 && index < list.Count)
            return true;
        return false;
    }

    ///// <summary>
    ///// 包含索引值
    ///// </summary>
    ///// <param name="list">列表</param>
    ///// <param name="index">索引</param>
    ///// <returns>包含为 <see langword="true"/> 不包含为 <see langword="false"/></returns>
    //public static bool ContainsIndex(this IList list, int index)
    //{
    //    ArgumentNullException.ThrowIfNull(list, nameof(list));
    //    if (index >= 0 && index < list.Count)
    //        return true;
    //    return false;
    //}
}
