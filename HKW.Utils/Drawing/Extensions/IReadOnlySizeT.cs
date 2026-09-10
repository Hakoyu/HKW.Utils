using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class ISizeTExtensions
{
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
        public static bool operator ==(IReadOnlySize<T> a, IReadOnlySize<T> b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        /// <summary>
        /// 大小不相等
        /// </summary>
        /// <param name="a">大小A</param>
        /// <param name="b">大小B</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(IReadOnlySize<T> a, IReadOnlySize<T> b)
        {
            return !(a == b);
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
            if (includeBoundary)
            {
                return x >= T.Zero && x <= size.Width && y >= T.Zero && y <= size.Height;
            }

            return x > T.Zero && x < size.Width && y > T.Zero && y < size.Height;
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
            ArgumentNullException.ThrowIfNull(rectangle);

            if (includeBoundary)
            {
                return rectangle.X >= T.Zero
                    && rectangle.Y >= T.Zero
                    && rectangle.RightBottom.X <= size.Width
                    && rectangle.RightBottom.Y <= size.Height;
            }

            return rectangle.X >= T.Zero
                && rectangle.Y >= T.Zero
                && rectangle.RightBottom.X < size.Width
                && rectangle.RightBottom.Y < size.Height;
        }
    }
}
