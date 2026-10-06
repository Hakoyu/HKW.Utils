using System.ComponentModel;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// <see cref="PropertyChangedEventArgs"/> 缓存扩展
/// </summary>
public static class PropertyChangedEventArgsExtensions
{
    extension(PropertyChangedEventArgs eventArgs)
    {
        /// <summary>
        /// 是索引器事件
        /// </summary>
        /// <returns>是否是索引器事件</returns>
        public bool IsIndexer()
        {
            return eventArgs.PropertyName == PropertyChangedArgsCache.Indexer1.PropertyName
                || eventArgs.PropertyName == PropertyChangedArgsCache.Indexer2.PropertyName;
        }
    }

    extension(PropertyChangedEventHandler eventHandler)
    {
        /// <summary>
        /// 触发索引器事件"Item[]"和"Item"
        /// </summary>
        /// <param name="source">源</param>
        public void InvokeIndexer(INotifyPropertyChanged source)
        {
            eventHandler(source, PropertyChangedArgsCache.Indexer1);
            eventHandler(source, PropertyChangedArgsCache.Indexer2);
        }
    }
}
