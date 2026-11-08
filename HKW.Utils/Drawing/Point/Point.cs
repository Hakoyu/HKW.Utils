using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 点
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public partial struct Point<T> : IEquatable<Point<T>>, IEquatable<IReadOnlyPoint<T>>, IPoint<T>
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
    {
        ArgumentNullException.ThrowIfNull(pt);
        this = new(pt.X, pt.Y);
    }

    /// <inheritdoc/>
    /// <param name="sz">大小</param>
    public Point(IReadOnlySize<T> sz)
    {
        ArgumentNullException.ThrowIfNull(sz);
        this = new(sz.Width, sz.Height);
    }

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
        return obj is Point<T> value ? Equals(value) : Equals(obj as IReadOnlyPoint<T>);
    }

    /// <inheritdoc/>
    public readonly bool Equals(IReadOnlyPoint<T>? other)
    {
        return other is not null
            && EqualityComparer<T>.Default.Equals(X, other.X)
            && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    /// <inheritdoc/>
    public readonly bool Equals(Point<T> other) =>
        EqualityComparer<T>.Default.Equals(X, other.X)
        && EqualityComparer<T>.Default.Equals(Y, other.Y);

    /// <summary>
    /// 按字段值比较, NaN 使用数值 Equals 语义.
    /// </summary>
    public static bool operator ==(Point<T> left, Point<T> right) => left.Equals(right);

    /// <summary>
    /// 按字段值判断不相等.
    /// </summary>
    public static bool operator !=(Point<T> left, Point<T> right) => left.Equals(right) is false;

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
    /// <remarks>
    /// <![CDATA[
    /// Parse("123,456",',')
    /// 返回:
    /// {X=123,Y=456}
    /// ]]>
    /// </remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Point<T> Parse(ReadOnlySpan<char> span, char separator = ',') =>
        Parse(span, separator, null);

    /// <summary>
    /// 按指定文化解析严格数量的数值字段; 不接受 ToString 的带标签格式.
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符; 小数逗号文化应使用其他分隔符. </param>
    /// <param name="provider">文化提供者, null 表示当前文化. </param>
    /// <returns>创建的几何值</returns>
    /// <exception cref="FormatException">字段数或数值格式不正确. </exception>
    public static Point<T> Parse(ReadOnlySpan<char> span, char separator, IFormatProvider? provider)
    {
        var x = GeometryMath<T>.ParseField(ref span, separator, false, provider);
        var y = GeometryMath<T>.ParseField(ref span, separator, true, provider);
        return new(x, y);
    }
}
