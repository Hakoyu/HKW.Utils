using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HKW.HKWUtils.Extensions;

/// <summary>
///
/// </summary>
public static class PropertyChangedEventArgsExtensions
{
    private static PropertyChangedEventArgs _count = new("Count");
    private static PropertyChangedEventArgs _indexer1 = new("Item[]");
    private static PropertyChangedEventArgs _indexer2 = new("Item");

    //private static PropertyChangedEventArgs _indexer
    extension(PropertyChangedEventArgs)
    {
        /// <summary>
        /// 属性 Count
        /// </summary>
        public static PropertyChangedEventArgs Cache_Count => _count;

        /// <summary>
        /// 索引器1, 用于反射绑定 (WPF, Avalonia等)
        /// </summary>
        public static PropertyChangedEventArgs Cache_Indexer1 => _indexer1;

        /// <summary>
        /// 索引器2, 用于编译绑定绑定 (Avalonia等)
        /// </summary>
        public static PropertyChangedEventArgs Cache_Indexer2 => _indexer2;
    }

    extension(PropertyChangedEventHandler value)
    {
        /// <summary>
        /// 触发索引器事件 "Item[]" 和 "Item"
        /// </summary>
        /// <param name="source">源</param>
        public void InvokeIndexer(INotifyPropertyChanged source)
        {
            value(source, PropertyChangedEventArgs.Cache_Indexer1);
            value(source, PropertyChangedEventArgs.Cache_Indexer2);
        }
    }
}
