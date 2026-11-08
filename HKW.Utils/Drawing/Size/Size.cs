using System.ComponentModel;
using System.Numerics;

namespace HKW.HKWUtils.Drawing;

/// <summary>
/// 大小
/// </summary>
/// <typeparam name="T">数值类型</typeparam>
/// <remarks>
/// 同类型相等及运算符使用数值 Equals 语义, NaN 与 NaN 相等; 哈希键入表后不可修改.
/// 跨实现比较按字段值; 外部实现须使用相同相等与哈希规则以保持对称性.
/// 哈希使用 HashCode.Combine, 字段顺序为 Width, Height.
/// 变更实例方法修改局部变量或数组元素; List 索引器返回副本, 修改后须显式写回.
/// IsEmpty 仅表示所有分量为零, 不表示数学上的空集或零面积.
/// 构造及尺寸 setter 拒绝负值; 允许存储 NaN, 但 Contains 对非有限或非法几何返回 false.
/// </remarks>
public partial struct Size<T> : IEquatable<Size<T>>, IEquatable<IReadOnlySize<T>>, ISize<T>
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
    {
        ArgumentNullException.ThrowIfNull(size);
        this = new(size.Width, size.Height);
    }
    #endregion

    /// <inheritdoc/>
    public T Width
    {
        readonly get => field;
        set
        {
            GeometryMath<T>.ValidateSize(value, nameof(Width));
            field = value;
        }
    }

    /// <inheritdoc/>
    public T Height
    {
        readonly get => field;
        set
        {
            GeometryMath<T>.ValidateSize(value, nameof(Height));
            field = value;
        }
    }

    /// <summary>
    /// 是空的
    /// </summary>
    [Browsable(false)]
    public readonly bool IsEmpty => Width == T.Zero && Height == T.Zero;

    #region Equals

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is Size<T> value ? Equals(value) : Equals(obj as IReadOnlySize<T>);
    }

    /// <inheritdoc/>
    public readonly bool Equals(IReadOnlySize<T>? other)
    {
        return other is not null
            && EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);
    }

    /// <inheritdoc/>
    public readonly bool Equals(Size<T> other) =>
        EqualityComparer<T>.Default.Equals(Width, other.Width)
            && EqualityComparer<T>.Default.Equals(Height, other.Height);

    /// <summary>
    /// 按字段值比较, NaN 使用数值 Equals 语义.
    /// </summary>
    public static bool operator ==(Size<T> left, Size<T> right) => left.Equals(right);

    /// <summary>
    /// 按字段值判断不相等.
    /// </summary>
    public static bool operator !=(Size<T> left, Size<T> right) => left.Equals(right) is false;

    #endregion
    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return $"{{Width={Width},Height={Height}}}";
    }

    /// <summary>
    /// 解析字符串数创建大小
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符</param>
    /// <returns>创建的大小</returns>
    /// <remarks>
    /// <![CDATA[
    /// Parse("123,456",',')
    /// 返回:
    /// {Width=123,Height=456}
    /// ]]>
    /// </remarks>
    /// <exception cref="Exception">数据 <paramref name="span"/> 解析错误</exception>
    public static Size<T> Parse(ReadOnlySpan<char> span, char separator = ',') =>
        Parse(span, separator, null);

    /// <summary>
    /// 按指定文化解析严格数量的数值字段; 不接受 ToString 的带标签格式.
    /// </summary>
    /// <param name="span">数据</param>
    /// <param name="separator">分割符; 小数逗号文化应使用其他分隔符. </param>
    /// <param name="provider">文化提供者, null 表示当前文化. </param>
    /// <returns>创建的几何值</returns>
    /// <exception cref="FormatException">字段数或数值格式不正确. </exception>
    public static Size<T> Parse(ReadOnlySpan<char> span, char separator, IFormatProvider? provider)
    {
        var width = GeometryMath<T>.ParseField(ref span, separator, false, provider);
        var height = GeometryMath<T>.ParseField(ref span, separator, true, provider);
        return new(width, height);
    }
}
