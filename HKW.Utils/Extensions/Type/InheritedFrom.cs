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
    /// 继承自基类类型
    /// </summary>
    /// <typeparam name="T">基类类型</typeparam>
    /// <param name="type">类型</param>
    /// <returns>当类型继承基类时为 <see langword="true"/>, 未继承为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InheritedFrom<T>(this Type type)
    {
        var t = type.BaseType;
        while (t is not null)
        {
            if (t == typeof(T))
                return true;
            t = t.BaseType;
        }
        return false;
    }

    /// <summary>
    /// 继承自基类类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="baseType">基类类型</param>
    /// <returns>当类型继承基类时为 <see langword="true"/>, 未继承为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InheritedFrom(this Type type, Type baseType)
    {
        var t = type.BaseType;
        while (t is not null)
        {
            if (t == baseType)
                return true;
            t = t.BaseType;
        }
        return false;
    }
}
