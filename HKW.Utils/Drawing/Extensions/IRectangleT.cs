using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IReadOnlyRectangleTExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="rectangle">源矩形</param>
    extension<T>(IRectangle<T> rectangle)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="otherRectangle">矩形</param>
        /// <exception cref="ArgumentNullException"><paramref name="otherRectangle"/> 为 null</exception>
        public void SetValue(IReadOnlyRectangle<T> otherRectangle)
        {
            ArgumentNullException.ThrowIfNull(otherRectangle);

            SetValue(
                rectangle,
                otherRectangle.X,
                otherRectangle.Y,
                otherRectangle.Width,
                otherRectangle.Height
            );
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="size">大小</param>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> 或 <paramref name="size"/> 为 null</exception>
        public void SetValue(IReadOnlyPoint<T> point, IReadOnlySize<T> size)
        {
            ArgumentNullException.ThrowIfNull(point);
            ArgumentNullException.ThrowIfNull(size);

            SetValue(rectangle, point.X, point.Y, size.Width, size.Height);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetValue(T x, T y, T width, T height)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
        }
    }
}
