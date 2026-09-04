using System.ComponentModel;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// <see cref="PropertyChangedEventArgs"/> 缓存扩展
/// </summary>
public static class PropertyChangedEventArgsExtensions
{
    private static readonly PropertyChangedEventArgs _count = new("Count");
    private static readonly PropertyChangedEventArgs _indexer1 = new("Item[]");
    private static readonly PropertyChangedEventArgs _indexer2 = new("Item");
    private static readonly PropertyChangedEventArgs _value = new("Value");
    private static readonly PropertyChangedEventArgs _x = new("X");
    private static readonly PropertyChangedEventArgs _y = new("Y");
    private static readonly PropertyChangedEventArgs _min = new("Min");
    private static readonly PropertyChangedEventArgs _max = new("Max");
    private static readonly PropertyChangedEventArgs _width = new("Width");
    private static readonly PropertyChangedEventArgs _height = new("Height");
    private static readonly PropertyChangedEventArgs _isEmpty = new("IsEmpty");
    private static readonly PropertyChangedEventArgs _left = new("Left");
    private static readonly PropertyChangedEventArgs _top = new("Top");
    private static readonly PropertyChangedEventArgs _right = new("Right");
    private static readonly PropertyChangedEventArgs _bottom = new("Bottom");
    private static readonly PropertyChangedEventArgs _leftTop = new("LeftTop");
    private static readonly PropertyChangedEventArgs _rightTop = new("RightTop");
    private static readonly PropertyChangedEventArgs _leftBottom = new("LeftBottom");
    private static readonly PropertyChangedEventArgs _rightBottom = new("RightBottom");
    private static readonly PropertyChangedEventArgs _state = new("State");
    private static readonly PropertyChangedEventArgs _stage = new("Stage");

    extension(PropertyChangedEventArgs)
    {
        /// <summary>
        /// 属性Count
        /// </summary>
        public static PropertyChangedEventArgs Cache_Count => _count;

        /// <summary>
        /// 索引器1，用于反射绑定（WPF、Avalonia等）
        /// </summary>
        public static PropertyChangedEventArgs Cache_Indexer1 => _indexer1;

        /// <summary>
        /// 索引器2，用于编译绑定（Avalonia等）
        /// </summary>
        public static PropertyChangedEventArgs Cache_Indexer2 => _indexer2;

        /// <summary>
        /// 属性Value
        /// </summary>
        public static PropertyChangedEventArgs Cache_Value => _value;

        /// <summary>
        /// 属性X
        /// </summary>
        public static PropertyChangedEventArgs Cache_X => _x;

        /// <summary>
        /// 属性Y
        /// </summary>
        public static PropertyChangedEventArgs Cache_Y => _y;

        /// <summary>
        /// 属性Min
        /// </summary>
        public static PropertyChangedEventArgs Cache_Min => _min;

        /// <summary>
        /// 属性Max
        /// </summary>
        public static PropertyChangedEventArgs Cache_Max => _max;

        /// <summary>
        /// 属性Width
        /// </summary>
        public static PropertyChangedEventArgs Cache_Width => _width;

        /// <summary>
        /// 属性Height
        /// </summary>
        public static PropertyChangedEventArgs Cache_Height => _height;

        /// <summary>
        /// 属性IsEmpty
        /// </summary>
        public static PropertyChangedEventArgs Cache_IsEmpty => _isEmpty;

        /// <summary>
        /// 属性Left
        /// </summary>
        public static PropertyChangedEventArgs Cache_Left => _left;

        /// <summary>
        /// 属性Top
        /// </summary>
        public static PropertyChangedEventArgs Cache_Top => _top;

        /// <summary>
        /// 属性Right
        /// </summary>
        public static PropertyChangedEventArgs Cache_Right => _right;

        /// <summary>
        /// 属性Bottom
        /// </summary>
        public static PropertyChangedEventArgs Cache_Bottom => _bottom;

        /// <summary>
        /// 属性LeftTop
        /// </summary>
        public static PropertyChangedEventArgs Cache_LeftTop => _leftTop;

        /// <summary>
        /// 属性RightTop
        /// </summary>
        public static PropertyChangedEventArgs Cache_RightTop => _rightTop;

        /// <summary>
        /// 属性LeftBottom
        /// </summary>
        public static PropertyChangedEventArgs Cache_LeftBottom => _leftBottom;

        /// <summary>
        /// 属性RightBottom
        /// </summary>
        public static PropertyChangedEventArgs Cache_RightBottom => _rightBottom;

        /// <summary>
        /// 属性Stage
        /// </summary>
        public static PropertyChangedEventArgs Cache_Stage => _stage;

        /// <summary>
        /// 属性State
        /// </summary>
        public static PropertyChangedEventArgs Cache_State => _state;
    }

    extension(PropertyChangedEventHandler value)
    {
        /// <summary>
        /// 触发索引器事件"Item[]"和"Item"
        /// </summary>
        /// <param name="source">源</param>
        public void InvokeIndexer(INotifyPropertyChanged source)
        {
            value(source, PropertyChangedEventArgs.Cache_Indexer1);
            value(source, PropertyChangedEventArgs.Cache_Indexer2);
        }
    }
}
