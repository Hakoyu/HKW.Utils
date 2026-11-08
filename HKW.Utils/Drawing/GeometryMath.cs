using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 几何数学
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
internal static class GeometryMath<T>
    where T : struct, INumber<T>
{
    internal static void ValidateSize(T value, string name)
    {
        if (value < T.Zero)
            throw new ArgumentOutOfRangeException(name, value, "Size must not be negative.");
    }

    internal static T End(T start, T length)
    {
        var end = checked(start + length);
        if (T.IsFinite(start) && T.IsFinite(length) && T.IsFinite(end) is false)
            throw new OverflowException("The geometry endpoint is not representable.");
        return end;
    }

    internal static bool TryEnd(T start, T length, out T end)
    {
        end = T.Zero;
        if (T.IsFinite(start) is false || T.IsFinite(length) is false || length < T.Zero)
            return false;
        try
        {
            end = checked(start + length);
            return T.IsFinite(end);
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    internal static bool ContainsRange(T min, T max, T otherMin, T otherMax, bool boundary)
    {
        if (
            T.IsFinite(min) is false
            || T.IsFinite(max) is false
            || T.IsFinite(otherMin) is false
            || T.IsFinite(otherMax) is false
            || min > max
            || otherMin > otherMax
        )
            return false;
        return boundary ? min <= otherMin && max >= otherMax : min < otherMin && max > otherMax;
    }

    internal static bool ContainsPoint(T x, T y, T width, T height, T px, T py, bool boundary)
    {
        if (
            TryEnd(x, width, out var right) is false
            || TryEnd(y, height, out var bottom) is false
            || T.IsFinite(px) is false
            || T.IsFinite(py) is false
        )
            return false;
        return boundary
            ? x <= px && y <= py && right >= px && bottom >= py
            : x < px && y < py && right > px && bottom > py;
    }

    internal static bool ContainsRectangle(
        T x,
        T y,
        T width,
        T height,
        T otherX,
        T otherY,
        T otherWidth,
        T otherHeight,
        bool boundary
    )
    {
        if (
            TryEnd(x, width, out var right) is false
            || TryEnd(y, height, out var bottom) is false
            || TryEnd(otherX, otherWidth, out var otherRight) is false
            || TryEnd(otherY, otherHeight, out var otherBottom) is false
        )
            return false;
        return boundary
            ? x <= otherX && y <= otherY && right >= otherRight && bottom >= otherBottom
            : x < otherX && y < otherY && right > otherRight && bottom > otherBottom;
    }

    internal static T ParseField(
        ref ReadOnlySpan<char> remaining,
        char separator,
        bool last,
        IFormatProvider? provider
    )
    {
        var index = remaining.IndexOf(separator);
        if (last)
        {
            if (index >= 0)
                throw new FormatException("The geometry contains too many fields.");
            return T.Parse(remaining, provider);
        }
        if (index < 0)
            throw new FormatException("The geometry contains too few fields.");
        var value = T.Parse(remaining[..index], provider);
        remaining = remaining[(index + 1)..];
        return value;
    }
}
