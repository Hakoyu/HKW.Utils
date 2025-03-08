using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 处理枚举中的全部项目并清空
    /// </summary>
    /// <typeparam name="T">可处理项目</typeparam>
    /// <param name="collection">集合</param>
    public static void DisposeAllAndClear<T>(this ICollection<T> collection)
        where T : IDisposable
    {
        foreach (var value in collection)
            value.Dispose();
        collection.Clear();
    }
}
