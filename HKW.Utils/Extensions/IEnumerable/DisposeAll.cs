using System.Collections;
using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 处理枚举中的全部项目
    /// </summary>
    /// <typeparam name="T">可处理项目</typeparam>
    /// <param name="values">集合</param>
    public static void DisposeAll<T>(this IEnumerable<T> values)
        where T : IDisposable
    {
        foreach (var value in values)
            value.Dispose();
    }
}
