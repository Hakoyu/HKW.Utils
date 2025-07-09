using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取成员的特性字典
    /// </summary>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="inherit">包括继承特性</param>
    /// <returns>特性字典</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeDictionary GetAttributeDictionary(
        this MemberInfo memberInfo,
        bool inherit = false
    )
    {
        return new AttributeDictionary(memberInfo, inherit);
    }
}
