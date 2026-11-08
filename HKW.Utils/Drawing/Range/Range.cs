using System.ComponentModel;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 范围
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public partial struct Range<T> : IEquatable<Range<T>>, IEquatable<IReadOnlyRange<T>>, IRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly Range<T> Empty;

    /// <inheritdoc/>
    /// <param name="range">范围</param>
    public Range(IReadOnlyRange<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        this = new(range.Min, range.Max);
    }

    /// <inheritdoc/>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public Range(T min, T max)
    {
        Min = min;
        Max = max;
    }

    /// <inheritdoc/>
    public T Min { readonly get; set; }

    /// <inheritdoc/>
    public T Max { readonly get; set; }

    /// <inheritdoc/>
    [Browsable(false)]
    public readonly bool IsEmpty => Min == T.Zero && Max == T.Zero;

    #region Equals

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is Range<T> value ? Equals(value) : Equals(obj as IReadOnlyRange<T>);
    }

    /// <inheritdoc/>
    public readonly bool Equals(IReadOnlyRange<T>? other)
    {
        return other is not null
            && EqualityComparer<T>.Default.Equals(Min, other.Min)
            && EqualityComparer<T>.Default.Equals(Max, other.Max);
    }

    /// <inheritdoc/>
    public readonly bool Equals(Range<T> other) =>
        EqualityComparer<T>.Default.Equals(Min, other.Min)
        && EqualityComparer<T>.Default.Equals(Max, other.Max);

    /// <summary>
    /// 按字段值比较, NaN 使用数值 Equals 语义.
    /// </summary>
    public static bool operator ==(Range<T> left, Range<T> right) => left.Equals(right);

    /// <summary>
    /// 按字段值判断不相等.
    /// </summary>
    public static bool operator !=(Range<T> left, Range<T> right) => left.Equals(right) is false;
    #endregion
    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return $"{{Min={Min},Max={Max}}}";
    }

    /// <summary>
    /// 解析字符串数创建范围
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的范围</returns>
    /// <remarks>
    /// <![CDATA[
    /// Parse("123,456",',')
    /// 返回:
    /// {Min=123,Max=456}
    /// ]]>
    /// </remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Range<T> Parse(ReadOnlySpan<char> span, char separator = ',') =>
        Parse(span, separator, null);

    /// <summary>
    /// 按指定文化解析严格数量的数值字段; 不接受 ToString 的带标签格式.
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符; 小数逗号文化应使用其他分隔符. </param>
    /// <param name="provider">文化提供者, null 表示当前文化. </param>
    /// <returns>创建的几何值</returns>
    /// <exception cref="FormatException">字段数或数值格式不正确. </exception>
    public static Range<T> Parse(ReadOnlySpan<char> span, char separator, IFormatProvider? provider)
    {
        var min = GeometryMath<T>.ParseField(ref span, separator, false, provider);
        var max = GeometryMath<T>.ParseField(ref span, separator, true, provider);
        return new(min, max);
    }
}
