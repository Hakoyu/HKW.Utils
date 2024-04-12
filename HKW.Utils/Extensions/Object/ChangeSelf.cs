using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 改变源自身
    /// </summary>
    /// <typeparam name="T">源类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="func">方法</param>
    /// <returns>改变后的源</returns>
    public static T ChangeSelf<T>(this T source, Func<T, T> func)
    {
        return func(source);
    }
}
