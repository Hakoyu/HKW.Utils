using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 点
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public struct Point<T> : IEquatable<IReadOnlyPoint<T>>, IPoint<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly Point<T> Empty;

    /// <inheritdoc/>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
    public Point(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    /// <param name="pt">点</param>
    public Point(IReadOnlyPoint<T> pt)
        : this(pt.X, pt.Y) { }

    /// <inheritdoc/>
    /// <param name="sz">大小</param>
    public Point(IReadOnlySize<T> sz)
        : this(sz.Width, sz.Height) { }

    /// <inheritdoc/>
    public T X { readonly get; set; }

    /// <inheritdoc/>
    public T Y { readonly get; set; }

    /// <inheritdoc/>
    [Browsable(false)]
    public readonly bool IsEmpty => X == T.Zero && Y == T.Zero;

    #region IEquatable
    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals(obj as IReadOnlyPoint<T>);
    }

    /// <inheritdoc/>
    public readonly bool Equals(IReadOnlyPoint<T>? other)
    {
        if (other is not Point<T> p)
            return false;
        return this == p;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    #endregion

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return $"{{X={X},Y={Y}}}";
    }

    /// <summary>
    /// 解析字符串数创建点
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的点</returns>
    /// <remarks><![CDATA[
    /// Parse("123,456",',')
    /// return:
    /// {X=123,Y=456}
    /// ]]></remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Point<T> Parse(ReadOnlySpan<char> span, char separator = ',')
    {
        var point = new Point<T>();
        var datas = span.Split(separator);
        datas.MoveNext();
        point.X = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        point.Y = T.Parse(span[datas.Current], null);
        return point;
    }
}
