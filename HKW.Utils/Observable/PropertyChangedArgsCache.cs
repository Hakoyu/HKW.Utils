using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HKW.HKWUtils;

#pragma warning disable CS1591
/// <summary>
/// <see cref="PropertyChangedEventArgs"/> 缓存
/// </summary>
public static class PropertyChangedArgsCache
{
    public static PropertyChangedEventArgs Info { get; } = new("Info");
    public static PropertyChangedEventArgs Count { get; } = new("Count");
    public static PropertyChangedEventArgs Value { get; } = new("Value");
    public static PropertyChangedEventArgs X { get; } = new("X");
    public static PropertyChangedEventArgs Y { get; } = new("Y");
    public static PropertyChangedEventArgs Min { get; } = new("Min");
    public static PropertyChangedEventArgs Max { get; } = new("Max");
    public static PropertyChangedEventArgs Width { get; } = new("Width");
    public static PropertyChangedEventArgs Height { get; } = new("Height");
    public static PropertyChangedEventArgs IsEmpty { get; } = new("IsEmpty");
    public static PropertyChangedEventArgs Left { get; } = new("Left");
    public static PropertyChangedEventArgs Top { get; } = new("Top");
    public static PropertyChangedEventArgs Right { get; } = new("Right");
    public static PropertyChangedEventArgs Bottom { get; } = new("Bottom");
    public static PropertyChangedEventArgs LeftTop { get; } = new("LeftTop");
    public static PropertyChangedEventArgs RightTop { get; } = new("RightTop");
    public static PropertyChangedEventArgs LeftBottom { get; } = new("LeftBottom");
    public static PropertyChangedEventArgs RightBottom { get; } = new("RightBottom");
    public static PropertyChangedEventArgs State { get; } = new("State");
    public static PropertyChangedEventArgs Stage { get; } = new("Stage");
    public static PropertyChangedEventArgs Indexer1 { get; } = new("Item");
    public static PropertyChangedEventArgs Indexer2 { get; } = new("Item[]");
}
#pragma warning restore CS1591
