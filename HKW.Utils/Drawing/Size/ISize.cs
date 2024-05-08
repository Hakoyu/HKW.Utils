using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 大小接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface ISize<T> : IReadOnlySize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 宽
    /// </summary>
    public new T Width { get; set; }

    /// <summary>
    /// 高
    /// </summary>
    public new T Height { get; set; }
}
