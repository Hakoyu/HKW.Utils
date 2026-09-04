using System.ComponentModel;

namespace HKW.HKWUtils.Extensions;

/// <summary>
/// <see cref="PropertyChangingEventArgs"/> 缓存扩展
/// </summary>
public static class PropertyChangingEventArgsExtensions
{
    private static readonly PropertyChangingEventArgs _value = new("Value");
    private static readonly PropertyChangingEventArgs _x = new("X");
    private static readonly PropertyChangingEventArgs _y = new("Y");
    private static readonly PropertyChangingEventArgs _min = new("Min");
    private static readonly PropertyChangingEventArgs _max = new("Max");
    private static readonly PropertyChangingEventArgs _width = new("Width");
    private static readonly PropertyChangingEventArgs _height = new("Height");
    private static readonly PropertyChangingEventArgs _isEmpty = new("IsEmpty");
    private static readonly PropertyChangingEventArgs _left = new("Left");
    private static readonly PropertyChangingEventArgs _top = new("Top");
    private static readonly PropertyChangingEventArgs _right = new("Right");
    private static readonly PropertyChangingEventArgs _bottom = new("Bottom");
    private static readonly PropertyChangingEventArgs _leftTop = new("LeftTop");
    private static readonly PropertyChangingEventArgs _rightTop = new("RightTop");
    private static readonly PropertyChangingEventArgs _leftBottom = new("LeftBottom");
    private static readonly PropertyChangingEventArgs _rightBottom = new("RightBottom");
    private static readonly PropertyChangingEventArgs _state = new("State");
    private static readonly PropertyChangingEventArgs _stage = new("Stage");

    extension(PropertyChangingEventArgs)
    {
        /// <summary>
        /// 属性Value
        /// </summary>
        public static PropertyChangingEventArgs Cache_Value => _value;

        /// <summary>
        /// 属性X
        /// </summary>
        public static PropertyChangingEventArgs Cache_X => _x;

        /// <summary>
        /// 属性Y
        /// </summary>
        public static PropertyChangingEventArgs Cache_Y => _y;

        /// <summary>
        /// 属性Min
        /// </summary>
        public static PropertyChangingEventArgs Cache_Min => _min;

        /// <summary>
        /// 属性Max
        /// </summary>
        public static PropertyChangingEventArgs Cache_Max => _max;

        /// <summary>
        /// 属性Width
        /// </summary>
        public static PropertyChangingEventArgs Cache_Width => _width;

        /// <summary>
        /// 属性Height
        /// </summary>
        public static PropertyChangingEventArgs Cache_Height => _height;

        /// <summary>
        /// 属性IsEmpty
        /// </summary>
        public static PropertyChangingEventArgs Cache_IsEmpty => _isEmpty;

        /// <summary>
        /// 属性Left
        /// </summary>
        public static PropertyChangingEventArgs Cache_Left => _left;

        /// <summary>
        /// 属性Top
        /// </summary>
        public static PropertyChangingEventArgs Cache_Top => _top;

        /// <summary>
        /// 属性Right
        /// </summary>
        public static PropertyChangingEventArgs Cache_Right => _right;

        /// <summary>
        /// 属性Bottom
        /// </summary>
        public static PropertyChangingEventArgs Cache_Bottom => _bottom;

        /// <summary>
        /// 属性LeftTop
        /// </summary>
        public static PropertyChangingEventArgs Cache_LeftTop => _leftTop;

        /// <summary>
        /// 属性RightTop
        /// </summary>
        public static PropertyChangingEventArgs Cache_RightTop => _rightTop;

        /// <summary>
        /// 属性LeftBottom
        /// </summary>
        public static PropertyChangingEventArgs Cache_LeftBottom => _leftBottom;

        /// <summary>
        /// 属性RightBottom
        /// </summary>
        public static PropertyChangingEventArgs Cache_RightBottom => _rightBottom;

        /// <summary>
        /// 属性Stage
        /// </summary>
        public static PropertyChangingEventArgs Cache_Stage => _stage;

        /// <summary>
        /// 属性State
        /// </summary>
        public static PropertyChangingEventArgs Cache_State => _state;
    }
}
