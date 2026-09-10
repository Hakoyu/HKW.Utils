using System.Runtime.CompilerServices;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class RangeExtensions
{
    /// <param name="range">范围</param>
    extension(Range range)
    {
        /// <summary>
        /// 获取偏移量
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>(起始索引, 结束索引)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Start, int End) GetOffset(int length)
        {
            int start = range.Start.GetOffset(length);
            int end = range.End.GetOffset(length);

            if ((uint)end > (uint)length || (uint)start > (uint)end)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, end);
        }
    }
}
