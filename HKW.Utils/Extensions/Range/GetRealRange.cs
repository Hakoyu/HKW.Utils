using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Extensions;

public static partial class HKWExtensions
{
    /// <summary>
    /// 获取偏移量
    /// </summary>
    /// <param name="range">范围</param>
    /// <param name="length">长度</param>
    /// <returns>(起始索引, 结束索引)</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Start, int End) GetOffset(this Range range, int length)
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
