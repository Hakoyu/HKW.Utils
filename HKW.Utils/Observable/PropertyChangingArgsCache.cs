using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HKW.HKWUtils;

#pragma warning disable CS1591
/// <summary>
/// <see cref="PropertyChangingEventArgs"/> 缓存
/// </summary>
public static class PropertyChangingArgsCache
{
    public static PropertyChangingEventArgs Info { get; } = new("Info");
    public static PropertyChangingEventArgs Count { get; } = new("Count");
    public static PropertyChangingEventArgs Value { get; } = new("Value");
    public static PropertyChangingEventArgs X { get; } = new("X");
    public static PropertyChangingEventArgs Y { get; } = new("Y");
    public static PropertyChangingEventArgs Min { get; } = new("Min");
    public static PropertyChangingEventArgs Max { get; } = new("Max");
    public static PropertyChangingEventArgs Width { get; } = new("Width");
    public static PropertyChangingEventArgs Height { get; } = new("Height");
    public static PropertyChangingEventArgs IsEmpty { get; } = new("IsEmpty");
    public static PropertyChangingEventArgs Left { get; } = new("Left");
    public static PropertyChangingEventArgs Top { get; } = new("Top");
    public static PropertyChangingEventArgs Right { get; } = new("Right");
    public static PropertyChangingEventArgs Bottom { get; } = new("Bottom");
    public static PropertyChangingEventArgs LeftTop { get; } = new("LeftTop");
    public static PropertyChangingEventArgs RightTop { get; } = new("RightTop");
    public static PropertyChangingEventArgs LeftBottom { get; } = new("LeftBottom");
    public static PropertyChangingEventArgs RightBottom { get; } = new("RightBottom");
    public static PropertyChangingEventArgs State { get; } = new("State");
    public static PropertyChangingEventArgs Stage { get; } = new("Stage");
}
#pragma warning restore CS1591
