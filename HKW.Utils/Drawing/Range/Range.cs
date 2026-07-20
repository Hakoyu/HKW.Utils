using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 范围
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public struct Range<T> : IEquatable<IReadOnlyRange<T>>, IRange<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly Range<T> Empty;

    /// <inheritdoc/>
    /// <param name="range">范围</param>
    public Range(IReadOnlyRange<T> range)
        : this(range.Min, range.Max) { }

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
    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyRange<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyRange<T>? other)
    {
        if (other is null)
            return false;
        return this == other;
    }
    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{Min={Min},Max={Max}}}";
    }

    /// <summary>
    /// 解析字符串数创建范围
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的范围</returns>
    /// <remarks><![CDATA[
    /// Parse("123,456",',')
    /// return:
    /// {Min=123,Max=456}
    /// ]]></remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Range<T> Parse(ReadOnlySpan<char> span, char separator = ',')
    {
        var range = new Range<T>();
        var datas = span.Split(separator);
        datas.MoveNext();
        range.Min = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        range.Max = T.Parse(span[datas.Current], null);
        return range;
    }
}
