using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
///
/// </summary>
public static partial class ISizeTExtensions
{
    /// <typeparam name="T">数值类型</typeparam>
    /// <param name="size">大小</param>
    extension<T>(ISize<T> size)
        where T : struct, INumber<T>
    {
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="otherSize">其他大小</param>
        /// <exception cref="ArgumentNullException"><paramref name="size"/> 或 <paramref name="otherSize"/> 为 null</exception>
        public void SetValue(IReadOnlySize<T> otherSize)
        {
            ArgumentNullException.ThrowIfNull(size);
            ArgumentNullException.ThrowIfNull(otherSize);
            SetValue(size, otherSize.Width, otherSize.Height);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetValue(T width, T height)
        {
            ArgumentNullException.ThrowIfNull(size);
            GeometryMath<T>.ValidateSize(width, nameof(width));
            GeometryMath<T>.ValidateSize(height, nameof(height));
            size.Width = width;
            size.Height = height;
        }
    }
}
