using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 删除
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T>(this IList<T> list, T item, out int index)
    {
        index = list.IndexOf(item);
        if (index < 0)
            return false;
        list.RemoveAt(index);
        return true;
    }
}
