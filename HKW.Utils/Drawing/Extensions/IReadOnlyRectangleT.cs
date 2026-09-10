using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IReadOnlyRectangleTExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="rectangle">矩形</param>
    extension<T>(IReadOnlyRectangle<T> rectangle)
        where T : struct, INumber<T>
    {
        /// <inheritdoc/>
        public static bool operator ==(IReadOnlyRectangle<T> a, IReadOnlyRectangle<T> b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        /// <inheritdoc/>
        public static bool operator !=(IReadOnlyRectangle<T> a, IReadOnlyRectangle<T> b)
        {
            return !(a == b);
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
            if (includeBoundary)
            {
                return rectangle.X <= x
                    && rectangle.Y <= y
                    && rectangle.Right >= x
                    && rectangle.Bottom >= y;
            }

            return rectangle.X < x
                && rectangle.Y < y
                && rectangle.Right > x
                && rectangle.Bottom > y;
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
            if (includeBoundary)
            {
                return rectangle.X <= x
                    && rectangle.Y <= y
                    && rectangle.Right >= unchecked(x + width)
                    && rectangle.Bottom >= unchecked(y + height);
            }

            return rectangle.X < x
                && rectangle.Y < y
                && rectangle.Right > unchecked(x + width)
                && rectangle.Bottom > unchecked(y + height);
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

            if (includeBoundary)
            {
                return rectangle.X <= otherRectangle.X
                    && rectangle.Y <= otherRectangle.Y
                    && rectangle.Right >= otherRectangle.Right
                    && rectangle.Bottom >= otherRectangle.Bottom;
            }

            return rectangle.X < otherRectangle.X
                && rectangle.Y < otherRectangle.Y
                && rectangle.Right > otherRectangle.Right
                && rectangle.Bottom > otherRectangle.Bottom;
        }
    }
}
