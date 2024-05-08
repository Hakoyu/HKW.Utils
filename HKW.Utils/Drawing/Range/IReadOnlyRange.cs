using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读范围接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IReadOnlyRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 最小值
    /// </summary>
    public T Min { get; }

    /// <summary>
    /// 最大值
    /// </summary>
    public T Max { get; }
}
