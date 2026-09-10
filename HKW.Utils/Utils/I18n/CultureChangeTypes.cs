using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 文化改变类型
/// </summary>
[Flags]
public enum CultureChangeTypes
{
    /// <summary>
    /// 不改变
    /// </summary>
    None = 0,

    /// <summary>
    /// <see cref="CultureInfo.CurrentCulture"/>
    /// </summary>
    CultureInfoCurrentCulture = 1 << 0,

    /// <summary>
    /// <see cref="CultureInfo.CurrentUICulture"/>
    /// </summary>
    CultureInfoCurrentUICulture = 1 << 1,
}
