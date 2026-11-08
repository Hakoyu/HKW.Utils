using System.Globalization;
using System.Numerics;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtilsTests.Drawing;

[TestClass]
public class GeometryInterfaceTests
{
    private sealed class ForeignPoint<T>(T x, T y) : IPoint<T>
        where T : struct, INumber<T>
    {
        public T X { get; set; } = x;
        public T Y { get; set; } = y;
        public bool IsEmpty => X == T.Zero && Y == T.Zero;

        public override bool Equals(object? obj) =>
            obj is IReadOnlyPoint<T> other && this.ValueEquals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    private sealed class ForeignSize<T>(T width, T height) : ISize<T>
        where T : struct, INumber<T>
    {
        public T Width { get; set; } = width;
        public T Height { get; set; } = height;
        public bool IsEmpty => Width == T.Zero && Height == T.Zero;

        public override bool Equals(object? obj) =>
            obj is IReadOnlySize<T> other && this.ValueEquals(other);

        public override int GetHashCode() => HashCode.Combine(Width, Height);
    }

    private sealed class ForeignRange<T>(T min, T max) : IRange<T>
        where T : struct, INumber<T>
    {
        public T Min { get; set; } = min;
        public T Max { get; set; } = max;
        public bool IsEmpty => Min == T.Zero && Max == T.Zero;

        public override bool Equals(object? obj) =>
            obj is IReadOnlyRange<T> other && this.ValueEquals(other);

        public override int GetHashCode() => HashCode.Combine(Min, Max);
    }

    private sealed class ForeignRectangle<T>(T x, T y, T width, T height) : IRectangle<T>
        where T : struct, INumber<T>
    {
        public T X { get; set; } = x;
        public T Y { get; set; } = y;
        public T Width { get; set; } = width;
        public T Height { get; set; } = height;
        public bool IsEmpty => X == T.Zero && Y == T.Zero && Width == T.Zero && Height == T.Zero;

        // 故意禁止读取派生端点，验证 Contains 依据基础字段安全计算，而不是信任外部实现。
        public T Left => throw new InvalidOperationException("Derived endpoint must not be read.");
        public T Top => throw new InvalidOperationException("Derived endpoint must not be read.");
        public T Right => throw new InvalidOperationException("Derived endpoint must not be read.");
        public T Bottom =>
            throw new InvalidOperationException("Derived endpoint must not be read.");
        public Point<T> LeftTop =>
            throw new InvalidOperationException("Derived endpoint must not be read.");
        public Point<T> RightTop =>
            throw new InvalidOperationException("Derived endpoint must not be read.");
        public Point<T> LeftBottom =>
            throw new InvalidOperationException("Derived endpoint must not be read.");
        public Point<T> RightBottom =>
            throw new InvalidOperationException("Derived endpoint must not be read.");

        public override bool Equals(object? obj) =>
            obj is IReadOnlyRectangle<T> other && this.ValueEquals(other);

        public override int GetHashCode() => HashCode.Combine(Width, Height, X, Y);
    }

    [TestMethod]
    public void CrossImplementationEqualityIsSymmetricIncludingNaNs()
    {
        var nan = BitConverter.Int64BitsToDouble(unchecked((long)0x7ff8000000000001UL));
        var point = new Point<double>(double.NaN, -0.0);
        var size = new Size<double>(double.NaN, -0.0);
        var range = new Range<double>(double.NaN, -0.0);
        var rectangle = new Rectangle<double>(double.NaN, -0.0, 1, 2);
        var fp = new ForeignPoint<double>(nan, 0);
        var fs = new ForeignSize<double>(nan, 0);
        var fr = new ForeignRange<double>(nan, 0);
        var frect = new ForeignRectangle<double>(nan, 0, 1, 2);
        Assert.IsTrue(point.Equals(fp) && fp.Equals(point));
        Assert.IsTrue(size.Equals(fs) && fs.Equals(size));
        Assert.IsTrue(range.Equals(fr) && fr.Equals(range));
        Assert.IsTrue(rectangle.Equals(frect) && frect.Equals(rectangle));
        Assert.IsTrue(point.Equals((object)fp));
        Assert.IsTrue(size.Equals((object)fs));
        Assert.IsTrue(range.Equals((object)fr));
        Assert.IsTrue(rectangle.Equals((object)frect));
        Assert.IsTrue(point.ValueEquals(fp) && fp.ValueEquals(point));
        Assert.IsTrue(size.ValueEquals(fs) && fs.ValueEquals(size));
        Assert.IsTrue(range.ValueEquals(fr) && fr.ValueEquals(range));
        Assert.IsTrue(rectangle.ValueEquals(frect) && frect.ValueEquals(rectangle));
        Assert.AreEqual(point.GetHashCode(), fp.GetHashCode());
        Assert.AreEqual(size.GetHashCode(), fs.GetHashCode());
        Assert.AreEqual(range.GetHashCode(), fr.GetHashCode());
        Assert.AreEqual(rectangle.GetHashCode(), frect.GetHashCode());
        Assert.IsTrue(point == new Point<double>(nan, 0));
        Assert.IsTrue(size == new Size<double>(nan, 0));
        Assert.IsTrue(range == new Range<double>(nan, 0));
        Assert.IsTrue(rectangle == new Rectangle<double>(nan, 0, 1, 2));
        Assert.IsFalse(point != new Point<double>(nan, 0));
        Assert.IsFalse(size != new Size<double>(nan, 0));
        Assert.IsFalse(range != new Range<double>(nan, 0));
        Assert.IsFalse(rectangle != new Rectangle<double>(nan, 0, 1, 2));
        Assert.IsTrue(point != new Point<double>(nan, 1));
        Assert.IsTrue(size != new Size<double>(nan, 1));
        Assert.IsTrue(range != new Range<double>(nan, 1));
        Assert.IsTrue(rectangle != new Rectangle<double>(nan, 0, 2, 2));
        Assert.IsFalse(point.Equals((object)size));
        Assert.IsFalse(size.Equals((object)range));
        Assert.IsFalse(range.Equals((object)rectangle));
        Assert.IsFalse(rectangle.Equals((object)point));
    }

    [TestMethod]
    public void InterfaceValueEqualsHasExplicitNullSemantics()
    {
        IReadOnlyPoint<int>? point = null;
        IReadOnlySize<int>? size = null;
        IReadOnlyRange<int>? range = null;
        IReadOnlyRectangle<int>? rectangle = null;
        Assert.IsTrue(point.ValueEquals(null));
        Assert.IsTrue(size.ValueEquals(null));
        Assert.IsTrue(range.ValueEquals(null));
        Assert.IsTrue(rectangle.ValueEquals(null));
        Assert.IsFalse(point.ValueEquals(Point<int>.Empty));
        Assert.IsFalse(size.ValueEquals(Size<int>.Empty));
        Assert.IsFalse(range.ValueEquals(Range<int>.Empty));
        Assert.IsFalse(rectangle.ValueEquals(Rectangle<int>.Empty));
        Assert.IsFalse(Point<int>.Empty.ValueEquals(point));
        Assert.IsFalse(Size<int>.Empty.ValueEquals(size));
        Assert.IsFalse(Range<int>.Empty.ValueEquals(range));
        Assert.IsFalse(Rectangle<int>.Empty.ValueEquals(rectangle));
        Assert.IsFalse(Point<int>.Empty.Equals(point));
        Assert.IsFalse(Size<int>.Empty.Equals(size));
        Assert.IsFalse(Range<int>.Empty.Equals(range));
        Assert.IsFalse(Rectangle<int>.Empty.Equals(rectangle));
        IReadOnlyPoint<int> first = new Point<int>(1, 2);
        IReadOnlyPoint<int> second = new Point<int>(1, 2);
        Assert.IsTrue(first.ValueEquals(second));
        Assert.IsFalse(ReferenceEquals(first, second));
        // 两个接口变量的 == 按 C# 预定义引用相等绑定，不能代替 ValueEquals。
        Assert.IsFalse(first == second);
    }

    [TestMethod]
    public void MixedValueInterfaceOperatorsUseValueEqualityAndHandleNull()
    {
        var point = new Point<double>(double.NaN, 0);
        var size = new Size<double>(double.NaN, 0);
        var range = new Range<double>(double.NaN, 0);
        var rectangle = new Rectangle<double>(double.NaN, 0, 1, 2);
        IReadOnlyPoint<double>? p = new ForeignPoint<double>(double.NaN, 0);
        IReadOnlySize<double>? s = new ForeignSize<double>(double.NaN, 0);
        IReadOnlyRange<double>? r = new ForeignRange<double>(double.NaN, 0);
        IReadOnlyRectangle<double>? rect = new ForeignRectangle<double>(double.NaN, 0, 1, 2);
        Assert.IsTrue(point == p && p == point);
        Assert.IsTrue(size == s && s == size);
        Assert.IsTrue(range == r && r == range);
        Assert.IsTrue(rectangle == rect && rect == rectangle);
        p = null;
        s = null;
        r = null;
        rect = null;
        Assert.IsFalse(point == p || p == point);
        Assert.IsFalse(size == s || s == size);
        Assert.IsFalse(range == r || r == range);
        Assert.IsFalse(rectangle == rect || rect == rectangle);
        Assert.IsTrue(point != p && p != point);
        Assert.IsTrue(size != s && s != size);
        Assert.IsTrue(range != r && r != range);
        Assert.IsTrue(rectangle != rect && rect != rectangle);
    }

    [TestMethod]
    public void ForeignMutationAndInvalidDimensionsAreCheckedBeforeAnyAssignment()
    {
        var p = new ForeignPoint<int>(1, 2);
        p.Offset(p);
        Assert.AreEqual(2, p.X);
        Assert.AreEqual(4, p.Y);
        p.SetValue(5, 6);
        var r = new ForeignRange<int>(0, 1);
        r.SetValue(new Range<int>(2, 3));
        Assert.AreEqual(3, r.Max);
        var size = new ForeignSize<int>(2, 3);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.SetValue(4, -1));
        Assert.AreEqual(2, size.Width);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            size.SetValue(new ForeignSize<int>(4, -1))
        );
        Assert.AreEqual(2, size.Width);
        var rect = new ForeignRectangle<int>(1, 2, 3, 4);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rect.SetValue(5, 6, 7, -1));
        Assert.AreEqual(1, rect.X);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            rect.SetValue(new ForeignRectangle<int>(5, 6, 7, -1))
        );
        Assert.AreEqual(1, rect.X);
        rect.SetValue(p, new Size<int>(7, 8));
        Assert.AreEqual(5, rect.X);
        Assert.AreEqual(8, rect.Height);
        var ownSize = new Size<int>(2, 3);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ownSize.SetValue(4, -1));
        Assert.AreEqual(2, ownSize.Width);
        var ownRect = new Rectangle<int>(1, 2, 3, 4);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ownRect.SetValue(5, 6, 7, -1));
        Assert.AreEqual(1, ownRect.X);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ownRect.SetValue(p, new ForeignSize<int>(7, -1))
        );
        Assert.AreEqual(1, ownRect.X);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new Size<int>(new ForeignSize<int>(1, -1))
        );
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new Rectangle<int>(new ForeignRectangle<int>(1, 2, -1, 4))
        );
    }

    [TestMethod]
    public void ContainsValidatesForeignFieldsAndIgnoresDerivedEndpoints()
    {
        var outer = new Rectangle<int>(0, 0, 10, 10);
        var size = new Size<int>(10, 10);
        IReadOnlyRectangle<int> foreignOuter = new ForeignRectangle<int>(0, 0, 10, 10);
        Assert.IsTrue(foreignOuter.Contains(1, 1));
        Assert.IsTrue(foreignOuter.Contains(outer, true));
        Assert.IsTrue(outer.Contains(foreignOuter, true));
        Assert.IsTrue(size.Contains(foreignOuter, true));
        foreach (
            var invalid in new[]
            {
                new ForeignRectangle<int>(20, 1, -15, 1),
                new ForeignRectangle<int>(1, 20, 1, -15),
                new ForeignRectangle<int>(int.MaxValue, 1, 1, 1),
                new ForeignRectangle<int>(1, int.MaxValue, 1, 1),
            }
        )
        {
            foreach (var boundary in new[] { false, true })
            {
                Assert.IsFalse(outer.Contains(invalid, boundary));
                Assert.IsFalse(((IReadOnlyRectangle<int>)outer).Contains(invalid, boundary));
                Assert.IsFalse(size.Contains(invalid, boundary));
                Assert.IsFalse(((IReadOnlySize<int>)size).Contains(invalid, boundary));
                Assert.IsFalse(invalid.Contains(1, 1, boundary));
            }
        }
        var negativeSize = new ForeignSize<int>(-1, 10);
        Assert.IsFalse(negativeSize.Contains(0, 0, true));
        Assert.IsFalse(negativeSize.Contains(new Rectangle<int>(), true));
        Assert.IsFalse(size.Contains(negativeSize, true));
        Assert.IsFalse(new ForeignRange<int>(10, 0).Contains(1, 2, true));
        Assert.IsTrue(new Rectangle<int>(-10, -10, 10, 10).Contains(-5, -5));
        Assert.IsTrue(new Range<int>(-10, 0).Contains(-5));
    }

    [TestMethod]
    public void EveryShapeArrayElementMutatesButListIndexerReturnsCopy()
    {
        var sizes = new[] { new Size<int>(1, 2) };
        var ranges = new[] { new Range<int>(1, 2) };
        var rectangles = new[] { new Rectangle<int>(1, 2, 3, 4) };
        sizes[0].SetValue(new Size<int>(3, 4));
        ranges[0].SetValue(new Range<int>(3, 4));
        rectangles[0].SetValue(new Point<int>(3, 4), new Size<int>(5, 6));
        Assert.AreEqual(4, sizes[0].Height);
        Assert.AreEqual(4, ranges[0].Max);
        Assert.AreEqual(6, rectangles[0].Height);
        var sizeList = new List<Size<int>> { sizes[0] };
        var rangeList = new List<Range<int>> { ranges[0] };
        var rectangleList = new List<Rectangle<int>> { rectangles[0] };
        sizeList[0].SetValue(8, 9);
        rangeList[0].SetValue(8, 9);
        rectangleList[0].SetValue(8, 9, 10, 11);
        Assert.AreEqual(4, sizeList[0].Height);
        Assert.AreEqual(4, rangeList[0].Max);
        Assert.AreEqual(6, rectangleList[0].Height);
        ISize<int> sizeBox = sizes[0];
        IRectangle<int> rectangleBox = rectangles[0];
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sizeBox.Height = -1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangleBox.Width = -1);
        Assert.AreEqual(4, sizeBox.Height);
        Assert.AreEqual(5, rectangleBox.Width);
    }

    [TestMethod]
    public void ParseUsesExplicitOrCurrentCultureAndDoesNotAcceptDisplayFormat()
    {
        var french = CultureInfo.GetCultureInfo("fr-FR");
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = french;
            Assert.AreEqual(new Point<decimal>(1.5m, 2.5m), Point<decimal>.Parse("1,5;2,5", ';'));
            Assert.AreEqual(new Size<decimal>(1.5m, 2.5m), Size<decimal>.Parse("1,5;2,5", ';'));
            Assert.AreEqual(new Range<decimal>(1.5m, 2.5m), Range<decimal>.Parse("1,5;2,5", ';'));
            Assert.AreEqual(
                new Rectangle<decimal>(1.5m, 2.5m, 3.5m, 4.5m),
                Rectangle<decimal>.Parse("1,5;2,5;3,5;4,5", ';')
            );
            Assert.AreEqual(
                new Point<decimal>(1.5m, 2.5m),
                Point<decimal>.Parse("1.5,2.5", ',', CultureInfo.InvariantCulture)
            );
            Assert.AreEqual(
                new Size<decimal>(1.5m, 2.5m),
                Size<decimal>.Parse("1.5,2.5", ',', CultureInfo.InvariantCulture)
            );
            Assert.AreEqual(
                new Range<decimal>(1.5m, 2.5m),
                Range<decimal>.Parse("1.5,2.5", ',', CultureInfo.InvariantCulture)
            );
            Assert.AreEqual(
                new Rectangle<decimal>(1.5m, 2.5m, 3.5m, 4.5m),
                Rectangle<decimal>.Parse("1.5,2.5,3.5,4.5", ',', CultureInfo.InvariantCulture)
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
        Assert.AreEqual(new Point<int>(-1, -2), Point<int>.Parse(" -1 ; -2 ", ';'));
        Assert.ThrowsExactly<FormatException>(() =>
            Point<int>.Parse(new Point<int>(1, 2).ToString())
        );
        Assert.ThrowsExactly<FormatException>(() =>
            Size<int>.Parse(new Size<int>(1, 2).ToString())
        );
        Assert.ThrowsExactly<FormatException>(() =>
            Range<int>.Parse(new Range<int>(1, 2).ToString())
        );
        Assert.ThrowsExactly<FormatException>(() =>
            Rectangle<int>.Parse(new Rectangle<int>(1, 2, 3, 4).ToString())
        );
        Assert.ThrowsExactly<OverflowException>(() => Point<byte>.Parse("256,1"));
        Assert.ThrowsExactly<OverflowException>(() => Range<uint>.Parse("-1,2"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Size<int>.Parse("-1,2"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Rectangle<int>.Parse("-1,-2,3,-4"));
        Assert.IsTrue(
            double.IsNaN(Point<double>.Parse("NaN,0", ',', CultureInfo.InvariantCulture).X)
        );
    }

    [TestMethod]
    public void NullArgumentsFailExplicitlyRatherThanDereferencing()
    {
        IReadOnlyPoint<int> point = null!;
        IReadOnlySize<int> size = null!;
        IReadOnlyRange<int> range = null!;
        IReadOnlyRectangle<int> rectangle = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => new Point<int>(point));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Point<int>(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Size<int>(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Range<int>(range));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Rectangle<int>(rectangle));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new Rectangle<int>(point, Size<int>.Empty)
        );
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new Rectangle<int>(Point<int>.Empty, size)
        );
        Assert.ThrowsExactly<ArgumentNullException>(() => Point<int>.Empty.Add(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => Point<int>.Empty.Subtract(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => Range<int>.Empty.Add(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => Range<int>.Empty.Subtract(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => point.Add(Size<int>.Empty));
        Assert.ThrowsExactly<ArgumentNullException>(() => range.Add(Size<int>.Empty));
        Assert.ThrowsExactly<ArgumentNullException>(() => size.Contains(0, 0));
        Assert.ThrowsExactly<ArgumentNullException>(() => rectangle.Contains(0, 0));
        Assert.ThrowsExactly<ArgumentNullException>(() => range.Contains(0));
        var p = new Point<int>();
        var s = new Size<int>();
        var r = new Range<int>();
        var rect = new Rectangle<int>();
        Assert.ThrowsExactly<ArgumentNullException>(() => p.SetValue(point));
        Assert.ThrowsExactly<ArgumentNullException>(() => p.Offset(point));
        Assert.ThrowsExactly<ArgumentNullException>(() => s.SetValue(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => r.SetValue(range));
        Assert.ThrowsExactly<ArgumentNullException>(() => rect.SetValue(rectangle));
        Assert.ThrowsExactly<ArgumentNullException>(() => rect.SetValue(point, s));
        Assert.ThrowsExactly<ArgumentNullException>(() => rect.SetValue(p, size));
        Assert.ThrowsExactly<ArgumentNullException>(() => s.Contains(point));
        Assert.ThrowsExactly<ArgumentNullException>(() => s.Contains(size));
        Assert.ThrowsExactly<ArgumentNullException>(() => s.Contains(rectangle));
        Assert.ThrowsExactly<ArgumentNullException>(() => r.Contains(range));
        Assert.ThrowsExactly<ArgumentNullException>(() => rect.Contains(point));
        Assert.ThrowsExactly<ArgumentNullException>(() => rect.Contains(rectangle));
        IPoint<int> mutablePoint = null!;
        ISize<int> mutableSize = null!;
        IRange<int> mutableRange = null!;
        IRectangle<int> mutableRectangle = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => mutablePoint.Offset(1, 2));
        Assert.ThrowsExactly<ArgumentNullException>(() => mutableSize.SetValue(1, 2));
        Assert.ThrowsExactly<ArgumentNullException>(() => mutableRange.SetValue(1, 2));
        Assert.ThrowsExactly<ArgumentNullException>(() => mutableRectangle.SetValue(1, 2, 3, 4));
    }
}
