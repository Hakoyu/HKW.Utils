﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 判断两个集合的值是否全部相等 (无视顺序)
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="collection1">集合1</param>
    /// <param name="collection2">集合2</param>
    /// <returns>相等为 <see langword="true"/> 不相等为 <see langword="false"/></returns>
    public static bool ItemsEqual<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
    {
        return collection1.Except(collection2).Any() is false;
    }
}
