﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 是否已定义特性
    /// </summary>
    /// <typeparam name="T">特性类型</typeparam>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="inherit">包括继承</param>
    /// <returns>已定义为 <see langword="true"/>, 否则为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(this MemberInfo memberInfo, bool inherit = false)
    {
        return memberInfo.IsDefined(typeof(T), inherit);
    }
}
