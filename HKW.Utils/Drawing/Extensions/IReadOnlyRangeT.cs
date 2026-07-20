using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IReadOnlyRangeTExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="range">范围</param>
    extension<T>(IReadOnlyRange<T> range)
        where T : struct, INumber<T>
    {
        /// <inheritdoc/>
        public static bool operator ==(IReadOnlyRange<T> a, IReadOnlyRange<T> b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <inheritdoc/>
        public static bool operator !=(IReadOnlyRange<T> a, IReadOnlyRange<T> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="rg">原范围</param>
        /// <param name="sz">添加量</param>
        /// <returns>添加后的新点</returns>
        public static Range<T> operator +(IReadOnlyRange<T> rg, IReadOnlySize<T> sz)
        {
            return Add(rg, sz);
        }

        /// <summary>
        /// 减少
        /// </summary>
        /// <param name="rg">原范围</param>
        /// <param name="sz">减少量</param>
        /// <returns>减少后的新点</returns>
        public static Range<T> operator -(IReadOnlyRange<T> rg, IReadOnlySize<T> sz)
        {
            return Subtract(rg, sz);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="size">添加量</param>
        /// <returns>添加后的新点</returns>
        public Range<T> Add(IReadOnlySize<T> size)
        {
            return new Range<T>(range.Min + size.Width, range.Max + size.Height);
        }

        /// <summary>
        /// 减少
        /// </summary>
        /// <param name="size">减少量</param>
        /// <returns></returns>
        public Range<T> Subtract(IReadOnlySize<T> size)
        {
            return new Range<T>(range.Min - size.Width, range.Max - size.Height);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="otherRange">其它范围</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherRange"/> 为 null</exception>
        public bool Contains(IReadOnlyRange<T> otherRange, bool includeBoundary = false)
        {
            ArgumentNullException.ThrowIfNull(otherRange);

            return Contains(range, otherRange.Min, otherRange.Max);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="includeBoundary">允许边界相等</param>
        /// <returns>是否包含</returns>
        public bool Contains(T min, T max, bool includeBoundary = false)
        {
            if (min > max)
                return false;

            if (includeBoundary)
                return range.Min <= min && range.Max >= max;

            return range.Min < min && range.Max > max;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否包含</returns>
        public bool Contains(T value)
        {
            return range.Max > value && range.Min < value;
        }
    }
}
