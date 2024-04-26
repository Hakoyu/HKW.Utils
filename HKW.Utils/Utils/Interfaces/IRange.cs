using System.Numerics;

namespace HKW.HKWUtils;

/// <summary>
/// 范围接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IRange<T>
    where T : INumber<T>
{
    /// <summary>
    /// 最小值
    /// </summary>
    public T Min { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public T Max { get; set; }
}
