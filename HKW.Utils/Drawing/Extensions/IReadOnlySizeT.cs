using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// Size 几何接口扩展.
/// </summary>
public static partial class ISizeTExtensions
{
    /// <summary>
    /// 按字段值比较任意实现; 两个 null 相等, NaN 使用数值 Equals 语义.
    /// </summary>
    /// <remarks>
    /// 接口变量的 == 可能绑定预定义引用相等; 需要值语义时显式调用本方法. 外部实现的 Equals 与哈希须遵循相同字段规则.
    /// </remarks>
    public static bool ValueEquals<T>(this IReadOnlySize<T>? value, IReadOnlySize<T>? other)
        where T : struct, INumber<T>
    {
        if (value is null)
            return other is null;
        return other is not null
            && EqualityComparer<T>.Default.Equals(value.Width, other.Width)
            && EqualityComparer<T>.Default.Equals(value.Height, other.Height);
    }

    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="size">大小</param>
    extension<T>(IReadOnlySize<T> size)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 大小相等
        /// </summary>
        /// <param name="a">大小A</param>
        /// <param name="b">大小B</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(IReadOnlySize<T>? a, IReadOnlySize<T>? b)
        {
            return ValueEquals(a, b);
        }

        /// <summary>
        /// 大小不相等
        /// </summary>
        /// <param name="a">大小A</param>
        /// <param name="b">大小B</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(IReadOnlySize<T>? a, IReadOnlySize<T>? b)
        {
            return ValueEquals(a, b) is false;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        public bool Contains(T x, T y, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(size);
            return GeometryMath<T>.ContainsPoint(
                T.Zero,
                T.Zero,
                size.Width,
                size.Height,
                x,
                y,
                includeBoundary
            );
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> 为 null</exception>
        public bool Contains(IReadOnlyPoint<T> point, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(point);
            return Contains(size, point.X, point.Y, includeBoundary);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="otherSize">其它大小</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherSize"/> 为 null</exception>
        /// <remarks>
        /// 按宽高比较尺寸向量, 即检查另一尺寸的右下端点; 不把共同原点作为严格边界排除.
        /// </remarks>
        public bool Contains(IReadOnlySize<T> otherSize, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(otherSize);
            return Contains(size, otherSize.Width, otherSize.Height, includeBoundary);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="rectangle">矩形</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rectangle"/> 为 null</exception>
        public bool Contains(IReadOnlyRectangle<T> rectangle, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(size);
            ArgumentNullException.ThrowIfNull(rectangle);
            return GeometryMath<T>.ContainsRectangle(
                T.Zero,
                T.Zero,
                size.Width,
                size.Height,
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height,
                includeBoundary
            );
        }
    }
}
