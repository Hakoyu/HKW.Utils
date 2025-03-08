using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 文化改变事件
/// </summary>
public class CultureChangedEventArgs : EventArgs
{
    /// <summary>
    /// 文化信息
    /// </summary>
    public CultureInfo CultureInfo { get; }

    /// <inheritdoc/>
    /// <param name="cultureInfo">文化信息</param>
    public CultureChangedEventArgs(CultureInfo cultureInfo)
    {
        CultureInfo = cultureInfo;
    }
}
