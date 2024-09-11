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
    /// 改变源自身
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="func">方法</param>
    /// <returns>改变后的源</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTarget To<TSource, TTarget>(this TSource source, Func<TSource, TTarget> func)
    {
        return func(source);
    }
}
