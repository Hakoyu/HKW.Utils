using System.Globalization;
using System.Numerics;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtilsTests.Drawing;

[TestClass]
public class GeometryContractTests
{
    [TestMethod]
    public void MutationsUpdateLocalsAndArrayElements()
    {
        var point = new Point<int>(1, 2);
        point.Offset(3, 4);
        Assert.AreEqual(4, point.X);
        point.SetValue(new Point<int>(8, 9));
        Assert.AreEqual(9, point.Y);
        var range = new Range<int>();
        range.SetValue(2, 8);
        Assert.AreEqual(8, range.Max);
        var size = new Size<int>();
        size.SetValue(3, 4);
        Assert.AreEqual(4, size.Height);
        var rectangle = new Rectangle<int>();
        rectangle.SetValue(1, 2, 3, 4);
        Assert.AreEqual(4, rectangle.Height);
        var points = new[] { point };
        points[0].Offset(1, 1);
        Assert.AreEqual(9, points[0].X);
    }

    [TestMethod]
    public void ExplicitBoxesAndListCopiesHaveExplicitOwnership()
    {
        var point = new Point<int>(1, 2);
        IPoint<int> box = point;
        box.Offset(3, 4);
        Assert.AreEqual(4, box.X);
        Assert.AreEqual(1, point.X);
        var list = new List<Point<int>> { point };
        list[0].Offset(3, 4);
        Assert.AreEqual(1, list[0].X);
        var copy = list[0];
        copy.Offset(3, 4);
        list[0] = copy;
        Assert.AreEqual(4, list[0].X);
    }

    [TestMethod]
    public void RangeBoundaryOverloadsAgree()
    {
        IReadOnlyRange<int> range = new Range<int>(0, 10);
        Assert.IsTrue(range.Contains(new Range<int>(0, 10), true));
        Assert.IsFalse(range.Contains(new Range<int>(0, 10), false));
        Assert.IsFalse(range.Contains(5, 4, true));
        Assert.IsFalse(new Range<int>(10, 0).Contains(1, 2, true));
    }

    [TestMethod]
    public void SizeRectangleStrictEdgesAreExcluded()
    {
        var size = new Size<int>(10, 10);
        foreach (
            var rectangle in new[]
            {
                new Rectangle<int>(0, 1, 1, 1),
                new Rectangle<int>(1, 0, 1, 1),
                new Rectangle<int>(9, 1, 1, 1),
                new Rectangle<int>(1, 9, 1, 1),
            }
        )
        {
            Assert.IsFalse(size.Contains(rectangle));
            Assert.IsTrue(size.Contains(rectangle, true));
        }
    }

    [TestMethod]
    public void NegativeDimensionsAreRejectedWithoutRejectingCoordinates()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Size<int>(-1, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Rectangle<int>(0, 0, 1, -1));
        var size = new Size<int>(1, 2);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Height = -1);
        var rectangle = new Rectangle<int>(-3, -4, 1, 2);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Width = -1);
        Assert.AreEqual(-3, rectangle.X);
        Assert.AreEqual(1, rectangle.Width);
        Assert.AreEqual(2, size.Height);
    }

    [TestMethod]
    public void InvalidRectangleAndOverflowAreNotContained()
    {
        var outer = new Rectangle<int>(0, 0, 10, 10);
        Assert.IsFalse(outer.Contains(20, 1, -15, 1, true));
        Assert.IsFalse(outer.Contains(int.MaxValue, 1, 1, 1, true));
        Assert.IsFalse(new Rectangle<int>(int.MaxValue, 0, 1, 1).Contains(0, 0, true));
        var unsigned = new Rectangle<uint>(0, 0, 10, 10);
        Assert.IsFalse(unsigned.Contains(uint.MaxValue, 1u, 2u, 1u, true));
        var decimals = new Rectangle<decimal>(0, 0, 10, 10);
        Assert.IsFalse(decimals.Contains(decimal.MaxValue, 1, 1, 1, true));
    }

    [TestMethod]
    public void NonFiniteGeometryIsNotContained()
    {
        Assert.IsFalse(
            new Rectangle<double>(0, 0, double.PositiveInfinity, 10).Contains(1, 1, true)
        );
        Assert.IsFalse(new Size<double>(double.PositiveInfinity, 10).Contains(1, 1, true));
        Assert.IsFalse(
            new Range<double>(double.NegativeInfinity, double.PositiveInfinity).Contains(0)
        );
        Assert.IsFalse(new Rectangle<double>(0, 0, 10, 10).Contains(double.NaN, 0, true));
    }

    [TestMethod]
    public void NaNEqualityIsReflexiveAndHashCompatible()
    {
        AssertValueEquality(new Point<double>(double.NaN, 0), new Point<double>(double.NaN, -0.0));
        AssertValueEquality(new Size<double>(double.NaN, 0), new Size<double>(double.NaN, -0.0));
        AssertValueEquality(new Range<double>(double.NaN, 0), new Range<double>(double.NaN, -0.0));
        AssertValueEquality(
            new Rectangle<double>(double.NaN, 0, 1, 1),
            new Rectangle<double>(double.NaN, -0.0, 1, 1)
        );
    }

    private static void AssertValueEquality<T>(T left, T right)
        where T : struct
    {
        Assert.IsTrue(EqualityComparer<T>.Default.Equals(left, right));
        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreEqual(1, new HashSet<T> { left, right }.Count);
    }

    [TestMethod]
    public void ConcreteTypesImplementSelfEquatable()
    {
        foreach (
            var type in new[]
            {
                typeof(Point<int>),
                typeof(Size<int>),
                typeof(Range<int>),
                typeof(Rectangle<int>),
            }
        )
            Assert.IsTrue(typeof(IEquatable<>).MakeGenericType(type).IsAssignableFrom(type));
    }

    [TestMethod]
    public void CrossImplementationPointEqualityUsesValues()
    {
        var point = new Point<int>(1, 2);
        Assert.IsTrue(point.Equals((IReadOnlyPoint<int>)new ForeignPoint<int>(1, 2)));
        Assert.IsFalse(point.Equals((IReadOnlyPoint<int>?)null));
    }

    private sealed record ForeignPoint<T>(T X, T Y) : IReadOnlyPoint<T>
        where T : struct, INumber<T>
    {
        public bool IsEmpty => X == T.Zero && Y == T.Zero;
    }

    [TestMethod]
    public void ReadOnlyAndThreeDimensionalContractsAreConsistent()
    {
        Assert.IsFalse(typeof(IReadOnlyRectangle<int>).GetProperty("X")!.CanWrite);
        Assert.IsFalse(typeof(IReadOnlyRectangle<int>).GetProperty("Y")!.CanWrite);
        Assert.IsTrue(typeof(IRectangle<int>).GetProperty("X")!.CanWrite);
        foreach (var name in new[] { "X", "Y", "Z", "SizeX", "SizeY", "SizeZ" })
            Assert.AreEqual(
                typeof(decimal),
                typeof(IRectangle3D<decimal>).GetProperty(name)!.PropertyType
            );
    }

    [TestMethod]
    public void ParseRejectsExtraMissingAndEmptyFields()
    {
        foreach (var text in new[] { "1", "1,2,3", "1,", ",2", "", "1,2," })
        {
            Assert.ThrowsExactly<FormatException>(() => Point<int>.Parse(text));
            Assert.ThrowsExactly<FormatException>(() => Size<int>.Parse(text));
            Assert.ThrowsExactly<FormatException>(() => Range<int>.Parse(text));
        }
        foreach (var text in new[] { "1,2,3", "1,2,3,4,5", "1,2,3,", "", "1,2,3,4," })
            Assert.ThrowsExactly<FormatException>(() => Rectangle<int>.Parse(text));
        Assert.AreEqual(2, Point<int>.Parse("1,2").Y);
        Assert.AreEqual(4, Rectangle<int>.Parse("1;2;3;4", ';').Height);
    }
}
