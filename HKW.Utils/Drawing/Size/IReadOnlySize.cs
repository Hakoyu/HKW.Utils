using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 只读大小接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IReadOnlySize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 宽
    /// </summary>
    public T Width { get; }

    /// <summary>
    /// 高
    /// </summary>
    public T Height { get; }
}
