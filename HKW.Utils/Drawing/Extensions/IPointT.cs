using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IPointExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="point">源点</param>
    extension<T>(IPoint<T> point)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="other">其他点</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 为 null</exception>
        public void SetValue(IReadOnlyPoint<T> other)
        {
            ArgumentNullException.ThrowIfNull(other);

            point.X = other.X;
            point.Y = other.Y;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        public void SetValue(T x, T y)
        {
            point.X = x;
            point.Y = y;
        }

        /// <summary>
        /// 偏移
        /// </summary>
        /// <param name="dx">偏移量X</param>
        /// <param name="dy">偏移量Y</param>
        public void Offset(T dx, T dy)
        {
            point.X += dx;
            point.Y += dy;
        }

        /// <summary>
        /// 偏移
        /// </summary>
        /// <param name="p">偏移量</param>
        public void Offset(IReadOnlyPoint<T> p)
        {
            Offset(point, p.X, p.Y);
        }
    }
}
