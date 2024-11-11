using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取全部信息
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public static string GetFullInfo(this CultureInfo cultureInfo)
    {
        return $"{cultureInfo.DisplayName} [{cultureInfo.Name}]";
    }
}
