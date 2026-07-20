using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 大小
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
public struct Size<T> : IEquatable<IReadOnlySize<T>>, ISize<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// 空
    /// </summary>
    public static readonly Size<T> Empty = new(default(T), default(T));

    #region ctor
    /// <inheritdoc/>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    /// <inheritdoc/>
    /// <param name="size">大小</param>
    public Size(IReadOnlySize<T> size)
        : this(size.Width, size.Height) { }
    #endregion

    /// <inheritdoc/>
    public T Width { readonly get; set; }

    /// <inheritdoc/>
    public T Height { readonly get; set; }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public readonly bool IsEmpty => Width == T.Zero && Height == T.Zero;

    #region Equals

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlySize<T>);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlySize<T>? other)
    {
        if (other is null)
            return false;
        return this == other;
    }

    #endregion
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{Width={Width},Height={Height}}}";
    }

    /// <summary>
    /// 解析字符串数创建大小
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的大小</returns>
    /// <remarks><![CDATA[
    /// Parse("123,456",',')
    /// return:
    /// {Width=123,Height=456}
    /// ]]></remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Size<T> Parse(ReadOnlySpan<char> span, char separator = ',')
    {
        var size = new Size<T>();
        var datas = span.Split(separator);
        datas.MoveNext();
        size.Width = T.Parse(span[datas.Current], null);
        datas.MoveNext();
        size.Height = T.Parse(span[datas.Current], null);
        return size;
    }
}
