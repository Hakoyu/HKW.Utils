using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class INumberExtensions
{
    extension<T>(T number)
        where T : INumber<T>
    {
        /// <summary>
        /// 数值的布尔值, 返回 number > T.Zero
        /// </summary>
        public bool BoolValue => number > T.Zero;
    }
}
