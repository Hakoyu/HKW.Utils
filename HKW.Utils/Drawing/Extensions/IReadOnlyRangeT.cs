using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// Range 几何接口扩展.
/// </summary>
/// <remarks>
/// Contains 对负尺寸, 反向范围, 非有限数值及不可表示端点返回 false
/// </remarks>
public static partial class IReadOnlyRangeTExtensions
{
    /// <summary>
    /// 按字段值比较任意实现; 两个 null 相等, NaN 使用数值 Equals 语义.
    /// </summary>
    /// <remarks>
    /// 接口变量的 == 可能绑定预定义引用相等; 需要值语义时显式调用本方法. 外部实现的 Equals 与哈希须遵循相同字段规则.
    /// </remarks>
    public static bool ValueEquals<T>(this IReadOnlyRange<T>? value, IReadOnlyRange<T>? other)
        where T : struct, INumber<T>
    {
        if (value is null)
            return other is null;
        return other is not null
            && EqualityComparer<T>.Default.Equals(value.Min, other.Min)
            && EqualityComparer<T>.Default.Equals(value.Max, other.Max);
    }

    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="range">范围</param>
    extension<T>(IReadOnlyRange<T> range)
        where T : struct, INumber<T>
    {
        /// <inheritdoc/>
        public static bool operator ==(IReadOnlyRange<T>? a, IReadOnlyRange<T>? b)
        {
            return ValueEquals(a, b);
        }

        /// <inheritdoc/>
        public static bool operator !=(IReadOnlyRange<T>? a, IReadOnlyRange<T>? b)
        {
            return ValueEquals(a, b) is false;
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
            ArgumentNullException.ThrowIfNull(range);
            ArgumentNullException.ThrowIfNull(size);
            return new Range<T>(
                unchecked(range.Min + size.Width),
                unchecked(range.Max + size.Height)
            );
        }

        /// <summary>
        /// 减少
        /// </summary>
        /// <param name="size">减少量</param>
        /// <returns></returns>
        public Range<T> Subtract(IReadOnlySize<T> size)
        {
            ArgumentNullException.ThrowIfNull(range);
            ArgumentNullException.ThrowIfNull(size);
            return new Range<T>(
                unchecked(range.Min - size.Width),
                unchecked(range.Max - size.Height)
            );
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

            return Contains(range, otherRange.Min, otherRange.Max, includeBoundary);
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
            ArgumentNullException.ThrowIfNull(range);
            return GeometryMath<T>.ContainsRange(range.Min, range.Max, min, max, includeBoundary);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否包含</returns>
        public bool Contains(T value)
        {
            ArgumentNullException.ThrowIfNull(range);
            return GeometryMath<T>.ContainsRange(range.Min, range.Max, value, value, false);
        }
    }
}
