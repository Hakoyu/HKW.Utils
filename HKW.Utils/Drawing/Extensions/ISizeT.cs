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
        /// <exception cref="ArgumentNullException"><paramref name="size"/> 为 null</exception>
        public void SetValue(IReadOnlySize<T> otherSize)
        {
            ArgumentNullException.ThrowIfNull(otherSize);
            size.Width = otherSize.Width;
            size.Height = otherSize.Height;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetValue(T width, T height)
        {
            size.Width = width;
            size.Height = height;
        }
    }
}
