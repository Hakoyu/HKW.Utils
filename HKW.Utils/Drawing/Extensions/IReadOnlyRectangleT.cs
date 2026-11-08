using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// Rectangle 几何接口扩展.
/// </summary>
public static partial class IReadOnlyRectangleTExtensions
{
    /// <summary>
    /// 按字段值比较任意实现; 两个 null 相等, NaN 使用数值 Equals 语义.
    /// </summary>
    /// <remarks>
    /// 接口变量的 == 可能绑定预定义引用相等; 需要值语义时显式调用本方法. 外部实现的 Equals 与哈希须遵循相同字段规则.
    /// </remarks>
    public static bool ValueEquals<T>(
        this IReadOnlyRectangle<T>? value,
        IReadOnlyRectangle<T>? other
    )
        where T : struct, INumber<T>
    {
        if (value is null)
            return other is null;
        return other is not null
            && EqualityComparer<T>.Default.Equals(value.X, other.X)
            && EqualityComparer<T>.Default.Equals(value.Y, other.Y)
            && EqualityComparer<T>.Default.Equals(value.Width, other.Width)
            && EqualityComparer<T>.Default.Equals(value.Height, other.Height);
    }

    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="rectangle">矩形</param>
    extension<T>(IReadOnlyRectangle<T> rectangle)
        where T : struct, INumber<T>
    {
        /// <inheritdoc/>
        public static bool operator ==(IReadOnlyRectangle<T>? a, IReadOnlyRectangle<T>? b)
        {
            return ValueEquals(a, b);
        }

        /// <inheritdoc/>
        public static bool operator !=(IReadOnlyRectangle<T>? a, IReadOnlyRectangle<T>? b)
        {
            return ValueEquals(a, b) is false;
        }

        /// <summary>
        /// 坐标在矩形内
        /// </summary>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否在内</returns>
        public bool Contains(T x, T y, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(rectangle);
            return GeometryMath<T>.ContainsPoint(
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height,
                x,
                y,
                includeBoundary
            );
        }

        /// <summary>
        /// 坐标在矩形内
        /// </summary>
        /// <param name="point">坐标</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否在内</returns>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> 为 null</exception>
        public bool Contains(IReadOnlyPoint<T> point, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(point);
            return Contains(rectangle, point.X, point.Y, includeBoundary);
        }

        /// <summary>
        /// 矩形在矩形内
        /// </summary>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否在内</returns>
        public bool Contains(T x, T y, T width, T height, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(rectangle);
            return GeometryMath<T>.ContainsRectangle(
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height,
                x,
                y,
                width,
                height,
                includeBoundary
            );
        }

        /// <summary>
        /// 矩形在矩形内
        /// </summary>
        /// <param name="otherRectangle">其它矩形</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否在内</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherRectangle"/> 为 null</exception>
        public bool Contains(IReadOnlyRectangle<T> otherRectangle, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(otherRectangle);
            return Contains(
                rectangle,
                otherRectangle.X,
                otherRectangle.Y,
                otherRectangle.Width,
                otherRectangle.Height,
                includeBoundary
            );
        }
    }
}
