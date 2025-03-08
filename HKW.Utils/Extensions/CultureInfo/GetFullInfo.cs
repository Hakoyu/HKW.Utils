using System.Globalization;

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
