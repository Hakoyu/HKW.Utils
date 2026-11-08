using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class CultureInfoExtensions
{
    extension(CultureInfo cultureInfo)
    {
        /// <summary>
        /// 获取全部信息
        /// </summary>
        /// <returns></returns>
        public string FullName
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"{cultureInfo.DisplayName} [{cultureInfo.Name}]";
        }

        /// <summary>
        /// 未知文化标识符
        /// </summary>
        public static string UnknownCulture => "UnknownCulture";
    }
    extension(CultureInfo c)
    {
        /// <summary>
        /// 检测文化是否存在
        /// </summary>
        /// <param name="name">文化名称</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string name)
        {
            return TryGetCultureInfo(name, out _);
        }

        /// <summary>
        /// 尝试获取文化信息
        /// </summary>
        /// <remarks>
        /// 空名称和空白名称返回 false, 因此不通过空名称获取 invariant 文化.
        /// 获取文化时的异常被捕获并返回 false, 失败时输出为 null.
        /// </remarks>
        /// <param name="name">文化名称</param>
        /// <param name="cultureInfo">文化信息</param>
        /// <returns>是否获取成功</returns>
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
}
