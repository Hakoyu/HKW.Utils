using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HKW.HKWUtils;

/// <summary>
/// 文化工具
/// </summary>
public static class CultureUtils
{
    /// <summary>
    /// 未知文化标识符
    /// </summary>
    public const string UnknownCulture = nameof(UnknownCulture);

    /// <summary>
    /// 检测文化是否存在
    /// </summary>
    /// <param name="name">文化名称</param>
    /// <returns>存在为 <see langword="true"/> 不存在为 <see langword="false"/></returns>
    public static bool Exists(string name)
    {
        return TryGetCultureInfo(name, out _);
    }

    /// <summary>
    /// 尝试获取文化信息
    /// </summary>
    /// <param name="name">文化名称</param>
    /// <param name="cultureInfo">文化信息</param>
    /// <returns>获取成功为 <see langword="true"/> 失败为 <see langword="false"/></returns>
    public static bool TryGetCultureInfo(
        string name,
        [MaybeNullWhen(false)] out CultureInfo cultureInfo
    )
    {
        cultureInfo = null;
        if (string.IsNullOrWhiteSpace(name))
            return false;
        try
        {
            cultureInfo = CultureInfo.GetCultureInfo(name, true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
