using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IPointExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="point">源点</param>
    extension<T>(IReadOnlyPoint<T> point)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 点相等
        /// </summary>
        /// <param name="a">点A</param>
        /// <param name="b">点B</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(IReadOnlyPoint<T> a, IReadOnlyPoint<T> b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// 点不相等
        /// </summary>
        /// <param name="a">点A</param>
        /// <param name="b">点B</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(IReadOnlyPoint<T> a, IReadOnlyPoint<T> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="pt">原点</param>
        /// <param name="sz">添加量</param>
        /// <returns>添加后的新点</returns>
        public static Point<T> operator +(IReadOnlyPoint<T> pt, IReadOnlySize<T> sz)
        {
            return Add(pt, sz);
        }

        /// <summary>
        /// 减少
        /// </summary>
        /// <param name="pt">原点</param>
        /// <param name="sz">减少量</param>
        /// <returns>减少后的新点</returns>
        public static Point<T> operator -(IReadOnlyPoint<T> pt, IReadOnlySize<T> sz)
        {
            return Subtract(pt, sz);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="size">添加量</param>
        /// <returns>添加后的新点</returns>
        public Point<T> Add(IReadOnlySize<T> size)
        {
            return new Point<T>(point.X + size.Width, point.Y + size.Height);
        }

        /// <summary>
        /// 减少
        /// </summary>
        /// <param name="size">减少量</param>
        /// <returns></returns>
        public Point<T> Subtract(IReadOnlySize<T> size)
        {
            return new Point<T>(point.X - size.Width, point.Y - size.Height);
        }
    }

    //public static Point<T> Ceiling(PointF value)
    //{
    //    return new Point<T>((int)Math.Ceiling(value.X), (int)Math.Ceiling(value.Y));
    //}

    //public static Point<T> Truncate(PointF value)
    //{
    //    return new Point<T>((int)value.X, (int)value.Y);
    //}

    //public static Point<T> Round(PointF value)
    //{
    //    return new Point<T>((int)Math.Round(value.X), (int)Math.Round(value.Y));
    //}
}
