using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class TupleExtensions
{
    extension<T1, T2>((T1, T2) tuple)
    {
        /// <inheritdoc/>
        public static bool operator ==((T1, T2) a, KeyValuePair<T1, T2> b)
        {
            return EqualityComparer<T1>.Default.Equals(a.Item1, b.Key)
                && EqualityComparer<T2>.Default.Equals(a.Item2, b.Value);
        }

        /// <inheritdoc/>
        public static bool operator !=((T1, T2) a, KeyValuePair<T1, T2> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 作为键值对
        /// </summary>
        /// <returns>键值对</returns>
        public KeyValuePair<T1, T2> AsPair()
        {
            return new KeyValuePair<T1, T2>(tuple.Item1, tuple.Item2);
        }
    }
}
