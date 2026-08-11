using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class NotifyCollectionChangedEventArgsExtensions
{
    private static NotifyCollectionChangedEventArgs _reset = new(
        NotifyCollectionChangedAction.Reset
    );
    extension(NotifyCollectionChangedEventArgs)
    {
        /// <summary>
        /// 重置
        /// </summary>
        public static NotifyCollectionChangedEventArgs Cache_Reset => _reset;
    }
}
