using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class MemberInfoExtensions
{
    /// <param name="memberInfo">成员信息</param>
    extension(MemberInfo memberInfo)
    {
        /// <summary>
        /// 获取成员的特性字典
        /// </summary>
        /// <param name="inherit">包括继承特性</param>
        /// <returns>特性字典</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AttributeDictionary GetAttributeDictionary(bool inherit = false)
        {
            return new AttributeDictionary(memberInfo, inherit);
        }

        /// <summary>
        /// 是否已定义特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="inherit">包括继承</param>
        /// <returns>是否已定义</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined<T>(bool inherit = false)
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }
    }
}
