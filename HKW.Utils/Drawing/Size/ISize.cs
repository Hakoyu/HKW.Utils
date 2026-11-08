using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 大小接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
/// <remarks>
/// 实现者的尺寸 setter 应拒绝负值; 只读接口可来自外部实现, Contains 会独立验证合法域.
/// </remarks>
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
