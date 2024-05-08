using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 范围接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IRange<T> : IReadOnlyRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 最小值
    /// </summary>
    public new T Min { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public new T Max { get; set; }
}
