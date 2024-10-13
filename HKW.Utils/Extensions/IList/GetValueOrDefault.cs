using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 使用索引获取列表的值或默认值
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="index">索引值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>获取成功则返回值, 获取失败则返回默认值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetValueOrDefault<T>(this IList<T> list, int index, T? defaultValue = default)
    {
        if (list.ContainsIndex(index))
            return list[index];
        return defaultValue;
    }
}
