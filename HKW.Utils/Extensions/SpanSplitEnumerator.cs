using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static partial class SpanSplitEnumeratorExtensions
{
    extension<T>(MemoryExtensions.SpanSplitEnumerator<T> enumerator)
        where T : IEquatable<T>
    {
        /// <summary>
        /// 从 <see cref="MemoryExtensions.SpanSplitEnumerator{T}.Current"/> 提供的 <see cref="Range"/> 获取的当前值
        /// </summary>
        public ReadOnlySpan<T> CurrentValue => enumerator.Source[enumerator.Current];

        /// <summary>
        /// 获取枚举中的当前分隔符。当枚举到最后一个元素时, 分隔符为 <see langword="default"/>(T)。
        /// </summary>
        public T CurrentSeparator =>
            enumerator.Current.End.Value > enumerator.Source.Length
                ? enumerator.Source[enumerator.Current.End]
                : default!;
    }
}
