using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Drawing;

namespace HKW.HKWUtilsTests.Drawing;

[TestClass]
public class GeometryNumericMatrixTests
{
    [TestMethod]
    [DataRow(typeof(sbyte))]
    [DataRow(typeof(byte))]
    [DataRow(typeof(short))]
    [DataRow(typeof(ushort))]
    [DataRow(typeof(int))]
    [DataRow(typeof(uint))]
    [DataRow(typeof(long))]
    [DataRow(typeof(ulong))]
    [DataRow(typeof(nint))]
    [DataRow(typeof(nuint))]
    [DataRow(typeof(Int128))]
    [DataRow(typeof(UInt128))]
    [DataRow(typeof(Half))]
    [DataRow(typeof(float))]
    [DataRow(typeof(double))]
    [DataRow(typeof(decimal))]
    [DataRow(typeof(BigInteger))]
    public void NumericMatrix(Type type) =>
        typeof(GeometryNumericMatrixTests)
            .GetMethod(nameof(CheckNumericMatrix), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(type)
            .Invoke(null, null);

    private static void CheckNumericMatrix<T>()
        where T : struct, INumber<T>
    {
        var z = T.Zero;
        var one = T.One;
        var two = T.CreateChecked(2);
        var three = T.CreateChecked(3);
        var four = T.CreateChecked(4);
        var ten = T.CreateChecked(10);
        var point = new Point<T>(one, two);
        var size = new Size<T>(one, two);
        var range = new Range<T>(one, two);
        var rectangle = new Rectangle<T>(one, two, three, four);
        Assert.AreEqual(point, new Point<T>(point));
        Assert.AreEqual(point, new Point<T>((IReadOnlyPoint<T>)point));
        Assert.AreEqual(point, new Point<T>(size));
        Assert.AreEqual(point, new Point<T>((IReadOnlySize<T>)size));
        Assert.AreEqual(size, new Size<T>(size));
        Assert.AreEqual(size, new Size<T>((IReadOnlySize<T>)size));
        Assert.AreEqual(range, new Range<T>(range));
        Assert.AreEqual(range, new Range<T>((IReadOnlyRange<T>)range));
        Assert.AreEqual(rectangle, new Rectangle<T>(rectangle));
        Assert.AreEqual(rectangle, new Rectangle<T>((IReadOnlyRectangle<T>)rectangle));
        Assert.AreEqual(rectangle, new Rectangle<T>(point, new Size<T>(three, four)));
        Assert.AreEqual(
            rectangle,
            new Rectangle<T>((IReadOnlyPoint<T>)point, new Size<T>(three, four))
        );
        Assert.AreEqual(point, Point<T>.Parse("1,2", ',', CultureInfo.InvariantCulture));
        Assert.AreEqual(size, Size<T>.Parse("1,2", ',', CultureInfo.InvariantCulture));
        Assert.AreEqual(range, Range<T>.Parse("1,2", ',', CultureInfo.InvariantCulture));
        Assert.AreEqual(
            rectangle,
            Rectangle<T>.Parse("1,2,3,4", ',', CultureInfo.InvariantCulture)
        );

        point.Offset(new Point<T>(one, one));
        Assert.AreEqual(new Point<T>(two, three), point);
        point.Offset((IReadOnlyPoint<T>)new Point<T>(one, one));
        Assert.AreEqual(new Point<T>(three, four), point);
        point.SetValue((IReadOnlyPoint<T>)new Point<T>(one, two));
        size.SetValue((IReadOnlySize<T>)new Size<T>(one, two));
        range.SetValue((IReadOnlyRange<T>)new Range<T>(one, two));
        rectangle.SetValue((IReadOnlyRectangle<T>)new Rectangle<T>(one, two, three, four));
        Assert.AreEqual(new Point<T>(two, four), point + size);
        Assert.AreEqual(point, (point + size) - size);
        Assert.AreEqual(new Range<T>(two, four), range + size);
        Assert.AreEqual(range, (range + size) - size);
        Assert.AreEqual(point.Add(size), ((IReadOnlyPoint<T>)point).Add(size));
        Assert.AreEqual(point.Subtract(size), ((IReadOnlyPoint<T>)point).Subtract(size));
        Assert.AreEqual(range.Add(size), ((IReadOnlyRange<T>)range).Add(size));
        Assert.AreEqual(range.Subtract(size), ((IReadOnlyRange<T>)range).Subtract(size));

        IPoint<T> pointBox = point;
        pointBox.Offset(new Point<T>(one, one));
        Assert.AreEqual(two, pointBox.X);
        pointBox.SetValue(new Point<T>(three, four));
        Assert.AreEqual(four, pointBox.Y);
        ISize<T> sizeBox = size;
        sizeBox.SetValue(new Size<T>(three, four));
        Assert.AreEqual(four, sizeBox.Height);
        IRange<T> rangeBox = range;
        rangeBox.SetValue(new Range<T>(three, four));
        Assert.AreEqual(four, rangeBox.Max);
        IRectangle<T> rectangleBox = rectangle;
        rectangleBox.SetValue(new Point<T>(z, z), new Size<T>(one, two));
        Assert.AreEqual(z, rectangleBox.X);
        rectangleBox.SetValue(new Rectangle<T>(two, three, four, ten));
        Assert.AreEqual(ten, rectangleBox.Height);

        var outer = new Rectangle<T>(z, z, ten, ten);
        IReadOnlyRectangle<T> outerInterface = outer;
        var bounds = new Size<T>(ten, ten);
        IReadOnlySize<T> boundsInterface = bounds;
        var interval = new Range<T>(z, ten);
        IReadOnlyRange<T> intervalInterface = interval;
        foreach (var boundary in new[] { false, true })
        {
            foreach (
                var p in new[]
                {
                    new Point<T>(z, one),
                    new Point<T>(one, z),
                    new Point<T>(ten, one),
                    new Point<T>(one, ten),
                    point,
                }
            )
            {
                var expected = outer.Contains(p.X, p.Y, boundary);
                Assert.AreEqual(expected, outer.Contains(p, boundary));
                Assert.AreEqual(expected, outer.Contains((IReadOnlyPoint<T>)p, boundary));
                Assert.AreEqual(expected, outerInterface.Contains(p, boundary));
                Assert.AreEqual(expected, outerInterface.Contains(p.X, p.Y, boundary));
                Assert.AreEqual(expected, bounds.Contains(p, boundary));
                Assert.AreEqual(expected, bounds.Contains((IReadOnlyPoint<T>)p, boundary));
                Assert.AreEqual(expected, boundsInterface.Contains(p, boundary));
                Assert.AreEqual(expected, boundsInterface.Contains(p.X, p.Y, boundary));
            }
            foreach (
                var r in new[]
                {
                    rectangle,
                    outer,
                    new Rectangle<T>(one, one, z, z),
                    new Rectangle<T>(z, z, z, z),
                }
            )
            {
                var expected = outer.Contains(r.X, r.Y, r.Width, r.Height, boundary);
                Assert.AreEqual(expected, outer.Contains(r, boundary));
                Assert.AreEqual(expected, outer.Contains((IReadOnlyRectangle<T>)r, boundary));
                Assert.AreEqual(expected, outerInterface.Contains(r, boundary));
                Assert.AreEqual(
                    expected,
                    outerInterface.Contains(r.X, r.Y, r.Width, r.Height, boundary)
                );
                Assert.AreEqual(expected, bounds.Contains(r, boundary));
                Assert.AreEqual(expected, bounds.Contains((IReadOnlyRectangle<T>)r, boundary));
                Assert.AreEqual(expected, boundsInterface.Contains(r, boundary));
            }
            Assert.AreEqual(boundary, interval.Contains(interval, boundary));
            Assert.AreEqual(boundary, interval.Contains((IReadOnlyRange<T>)interval, boundary));
            Assert.AreEqual(boundary, intervalInterface.Contains(interval, boundary));
            Assert.AreEqual(boundary, intervalInterface.Contains(z, ten, boundary));
            Assert.AreEqual(boundary, bounds.Contains(bounds, boundary));
            Assert.AreEqual(boundary, bounds.Contains((IReadOnlySize<T>)bounds, boundary));
            Assert.AreEqual(boundary, boundsInterface.Contains(bounds, boundary));
            Assert.IsTrue(interval.Contains(one, one, boundary));
            Assert.IsFalse(interval.Contains(two, one, boundary));
        }
        Assert.IsTrue(interval.Contains(one));
        Assert.IsTrue(intervalInterface.Contains(one));
        Assert.IsFalse(interval.Contains(z));
        Assert.AreEqual(four, rectangle.Right);
        Assert.AreEqual(T.CreateChecked(6), rectangle.Bottom);
        Assert.AreEqual(new Point<T>(one, two), rectangle.LeftTop);
        Assert.AreEqual(new Point<T>(four, two), rectangle.RightTop);
        Assert.AreEqual(new Point<T>(one, T.CreateChecked(6)), rectangle.LeftBottom);
        Assert.AreEqual(new Point<T>(four, T.CreateChecked(6)), rectangle.RightBottom);
        Assert.IsTrue(Point<T>.Empty.IsEmpty);
        Assert.IsTrue(Size<T>.Empty.IsEmpty);
        Assert.IsTrue(Range<T>.Empty.IsEmpty);
        Assert.IsTrue(Rectangle<T>.Empty.IsEmpty);
        Assert.IsFalse(new Size<T>(z, one).IsEmpty);
        Assert.IsFalse(new Range<T>(one, one).IsEmpty);
        Assert.IsTrue(Rectangle<T>.Empty.Contains(z, z, true));
        Assert.IsFalse(Rectangle<T>.Empty.Contains(z, z));
        Assert.IsTrue(Size<T>.Empty.Contains(z, z, true));
        Assert.IsFalse(Size<T>.Empty.Contains(z, z));
    }

    [TestMethod]
    public void CheckedEndpointsRejectEveryBoundedNumericOverflow()
    {
        CheckOverflow<byte>();
        CheckOverflow<sbyte>();
        CheckOverflow<short>();
        CheckOverflow<ushort>();
        CheckOverflow<int>();
        CheckOverflow<uint>();
        CheckOverflow<long>();
        CheckOverflow<ulong>();
        CheckOverflow<nint>();
        CheckOverflow<nuint>();
        CheckOverflow<Int128>();
        CheckOverflow<UInt128>();
        CheckOverflow<decimal>();
        CheckOverflow<Half>();
        CheckOverflow<float>();
        CheckOverflow<double>();
        var huge = BigInteger.One << 1024;
        Assert.AreEqual(huge * 2, new Rectangle<BigInteger>(huge, huge, huge, huge).Right);
    }

    private static void CheckOverflow<T>()
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var outer = new Rectangle<T>(T.Zero, T.Zero, T.MaxValue, T.MaxValue);
        var overflow = new Rectangle<T>(T.MaxValue, T.MaxValue, T.MaxValue, T.MaxValue);
        Assert.IsFalse(outer.Contains(overflow, true));
        Assert.IsFalse(((IReadOnlyRectangle<T>)outer).Contains(overflow, true));
        Assert.IsFalse(overflow.Contains(T.MaxValue, T.MaxValue, true));
        Assert.IsFalse(new Size<T>(T.MaxValue, T.MaxValue).Contains(overflow, true));
        Assert.ThrowsExactly<OverflowException>(() => _ = overflow.Right);
        Assert.ThrowsExactly<OverflowException>(() => _ = overflow.Bottom);
        Assert.ThrowsExactly<OverflowException>(() => _ = overflow.RightBottom);
        Assert.AreEqual(T.MaxValue, outer.Right);
        Assert.IsTrue(outer.Contains(T.MaxValue, T.MaxValue, true));
    }

    [TestMethod]
    public void DefaultArithmeticRemainsUncheckedAndDecimalFailureDoesNotPartiallyOffset()
    {
        checked
        {
            var point = new Point<int>(int.MaxValue, 1);
            point.Offset(1, 0);
            Assert.AreEqual(int.MinValue, point.X);
            Assert.AreEqual(
                int.MinValue,
                (new Point<int>(int.MaxValue, 0) + new Size<int>(1, 0)).X
            );
            Assert.AreEqual(
                byte.MinValue,
                (new Range<byte>(byte.MaxValue, byte.MaxValue) + new Size<byte>(1, 1)).Min
            );
            Assert.AreEqual(uint.MaxValue, (new Point<uint>(0, 0) - new Size<uint>(1, 0)).X);
        }
        var decimals = new Point<decimal>(1, decimal.MaxValue);
        Assert.ThrowsExactly<OverflowException>(() => decimals.Offset(1, 1));
        Assert.AreEqual(1m, decimals.X);
        IPoint<decimal> box = decimals;
        Assert.ThrowsExactly<OverflowException>(() => box.Offset(1m, 1m));
        Assert.AreEqual(1m, box.X);
        Assert.ThrowsExactly<OverflowException>(() =>
            _ = new Range<decimal>(1, decimal.MaxValue).Add(new Size<decimal>(1, 1))
        );
    }

    [TestMethod]
    public void FloatingSpecialValuesHaveSeparateEqualityAndContainmentSemantics()
    {
        CheckFloating<Half>();
        CheckFloating<float>();
        CheckFloating<double>();
    }

    private static void CheckFloating<T>()
        where T : struct, IFloatingPointIeee754<T>
    {
        var zero = T.Zero;
        var one = T.One;
        var nanPoint = new Point<T>(T.NaN, zero);
        var nanSize = new Size<T>(T.NaN, zero);
        var nanRange = new Range<T>(T.NaN, zero);
        var nanRectangle = new Rectangle<T>(T.NaN, zero, one, one);
        Assert.IsTrue(nanPoint.Equals(new Point<T>(T.NaN, T.NegativeZero)));
        Assert.IsTrue(nanSize.Equals(new Size<T>(T.NaN, T.NegativeZero)));
        Assert.IsTrue(nanRange.Equals(new Range<T>(T.NaN, T.NegativeZero)));
        Assert.IsTrue(nanRectangle.Equals(new Rectangle<T>(T.NaN, T.NegativeZero, one, one)));
        Assert.AreEqual(nanPoint.GetHashCode(), new Point<T>(T.NaN, T.NegativeZero).GetHashCode());
        Assert.AreEqual(nanSize.GetHashCode(), new Size<T>(T.NaN, T.NegativeZero).GetHashCode());
        Assert.AreEqual(nanRange.GetHashCode(), new Range<T>(T.NaN, T.NegativeZero).GetHashCode());
        Assert.AreEqual(
            nanRectangle.GetHashCode(),
            new Rectangle<T>(T.NaN, T.NegativeZero, one, one).GetHashCode()
        );
        foreach (var invalid in new[] { T.NaN, T.PositiveInfinity, T.NegativeInfinity })
        {
            var outer = new Rectangle<T>(zero, zero, one, one);
            Assert.IsFalse(outer.Contains(invalid, zero, true));
            Assert.IsFalse(outer.Contains(zero, invalid, true));
            Assert.IsFalse(outer.Contains(zero, zero, invalid, one, true));
            Assert.IsFalse(outer.Contains(zero, zero, one, invalid, true));
            Assert.IsFalse(new Rectangle<T>(invalid, zero, one, one).Contains(zero, zero, true));
            Assert.IsFalse(new Range<T>(invalid, one).Contains(zero));
            Assert.IsFalse(new Range<T>(zero, one).Contains(invalid, invalid, true));
        }
        Assert.IsFalse(nanSize.Contains(zero, zero, true));
        Assert.IsFalse(new Size<T>(T.PositiveInfinity, one).Contains(one, one, true));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new Size<T>(T.NegativeInfinity, zero)
        );
        Assert.AreEqual(zero, new Size<T>(T.NegativeZero, zero).Width);
    }

    [TestMethod]
    public void AllSignedDimensionEntryPointsRejectNegativesAndKeepState()
    {
        CheckNegative<sbyte>();
        CheckNegative<short>();
        CheckNegative<int>();
        CheckNegative<long>();
        CheckNegative<nint>();
        CheckNegative<Int128>();
        CheckNegative<BigInteger>();
        CheckNegative<Half>();
        CheckNegative<float>();
        CheckNegative<double>();
        CheckNegative<decimal>();
        var minimum = new Rectangle<int>(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
        Assert.AreEqual(-1, minimum.Right);
        Assert.IsTrue(minimum.Contains(int.MinValue, int.MinValue, true));
        Assert.IsTrue(minimum.Contains(-1, -1, true));
        Assert.IsFalse(minimum.Contains(0, 0, true));
    }

    private static void CheckNegative<T>()
        where T : struct, INumber<T>
    {
        var minusOne = T.CreateChecked(-1);
        var one = T.One;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Size<T>(minusOne, one));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Size<T>(one, minusOne));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new Rectangle<T>(minusOne, minusOne, minusOne, one)
        );
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new Rectangle<T>(minusOne, minusOne, one, minusOne)
        );
        var size = new Size<T>(one, one);
        var rectangle = new Rectangle<T>(minusOne, minusOne, one, one);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Width = minusOne);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Height = minusOne);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Width = minusOne);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Height = minusOne);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.SetValue(minusOne, one));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.SetValue(one, minusOne));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            rectangle.SetValue(one, one, minusOne, one)
        );
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            rectangle.SetValue(one, one, one, minusOne)
        );
        Assert.AreEqual(one, size.Width);
        Assert.AreEqual(one, size.Height);
        Assert.AreEqual(minusOne, rectangle.X);
        Assert.AreEqual(minusOne, rectangle.Y);
        Assert.AreEqual(one, rectangle.Width);
        Assert.AreEqual(one, rectangle.Height);
    }

    [TestMethod]
    public void ConcreteEqualityAndMutationsAllocateNoBoxes()
    {
        ExerciseValuePath<int>(1000);
        ExerciseValuePath<double>(1000);
        ExerciseValuePath<decimal>(1000);
        var before = GC.GetAllocatedBytesForCurrentThread();
        var result =
            ExerciseValuePath<int>(10000)
            && ExerciseValuePath<double>(10000)
            && ExerciseValuePath<decimal>(10000);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.IsTrue(result);
        Assert.AreEqual(0L, allocated);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool ExerciseValuePath<T>(int count)
        where T : struct, INumber<T>
    {
        var point = new Point<T>(T.One, T.One);
        var size = new Size<T>(T.One, T.One);
        var range = new Range<T>(T.Zero, T.One);
        var rectangle = new Rectangle<T>(T.Zero, T.Zero, T.One, T.One);
        var result = true;
        for (var i = 0; i < count; i++)
        {
            point.SetValue(point);
            point.Offset(T.Zero, T.Zero);
            size.SetValue(size);
            range.SetValue(range);
            rectangle.SetValue(rectangle);
            result &=
                EqualityComparer<Point<T>>.Default.Equals(point, point)
                && point == new Point<T>(size);
            result &=
                EqualityComparer<Size<T>>.Default.Equals(size, size) && size == new Size<T>(size);
            result &=
                EqualityComparer<Range<T>>.Default.Equals(range, range)
                && range == new Range<T>(range);
            result &=
                EqualityComparer<Rectangle<T>>.Default.Equals(rectangle, rectangle)
                && rectangle == new Rectangle<T>(rectangle);
            result &=
                (point + size).Subtract(size).Equals(point)
                && (range + size).Subtract(size).Equals(range);
            result &=
                range.Contains(range, true)
                && size.Contains(size, true)
                && rectangle.Contains(rectangle, true);
        }
        return result;
    }
}
