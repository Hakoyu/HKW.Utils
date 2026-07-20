using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class IReadOnlyRangeTExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="range">范围</param>
    extension<T>(IRange<T> range)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="otherRange">其他范围</param>
        /// <exception cref="ArgumentNullException"><paramref name="otherRange"/> 为 null</exception>
        public void SetValue(IReadOnlyRange<T> otherRange)
        {
            ArgumentNullException.ThrowIfNull(otherRange);
            SetValue(range, otherRange.Min, otherRange.Max);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public void SetValue(T min, T max)
        {
            range.Min = min;
            range.Max = max;
        }
    }
}
