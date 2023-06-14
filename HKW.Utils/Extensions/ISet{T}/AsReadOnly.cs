using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 将普通集合转换为只读集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="this">集合</param>
    /// <returns>只读集合</returns>
    public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> @this)
        where T : notnull
    {
        return new ReadOnlySet<T>(@this);
    }
}
