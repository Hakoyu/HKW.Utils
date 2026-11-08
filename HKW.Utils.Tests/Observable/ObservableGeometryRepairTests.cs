using System.Globalization;
using System.Numerics;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableGeometryRepairTests
{
    [TestMethod]
    public void NumericTypesShareGeometryContracts()
    {
        CheckNumeric<int>();
        CheckNumeric<uint>();
        CheckNumeric<long>();
        CheckNumeric<ulong>();
        CheckNumeric<byte>();
        CheckNumeric<sbyte>();
        CheckNumeric<short>();
        CheckNumeric<ushort>();
        CheckNumeric<float>();
        CheckNumeric<double>();
        CheckNumeric<nint>();
        CheckNumeric<nuint>();
        CheckNumeric<Int128>();
        CheckNumeric<UInt128>();
        CheckNumeric<Half>();
        CheckNumeric<decimal>();
        CheckNumeric<BigInteger>();
    }

    private static void CheckNumeric<T>()
        where T : struct, INumber<T>
    {
        var zero = T.Zero;
        var one = T.One;
        var two = T.CreateChecked(2);
        var ten = T.CreateChecked(10);
        var point = new ObservablePoint<T>(one, two);
        var size = new ObservableSize<T>(ten, ten);
        var range = new ObservableRange<T>(zero, ten);
        var rectangle = new ObservableRectangle<T>(zero, zero, ten, ten);
        Assert.IsTrue(point.Equals(new Point<T>(one, two)));
        Assert.IsTrue(size.Equals((object)new Size<T>(ten, ten)));
        Assert.IsTrue(range.Equals(new HKW.HKWUtils.Drawing.Range<T>(zero, ten)));
        Assert.IsTrue(rectangle.Equals(new Rectangle<T>(zero, zero, ten, ten)));
        Assert.AreEqual(
            new Rectangle<T>(zero, zero, ten, ten).GetHashCode(),
            rectangle.GetHashCode()
        );
        Assert.IsTrue(rectangle.Contains(new Point<T>(one, two)));
        Assert.IsFalse(rectangle.Contains(new Point<T>(zero, two)));
        Assert.IsTrue(rectangle.Contains(new Point<T>(zero, two), true));
        Assert.IsFalse(size.Contains(new Rectangle<T>(zero, one, one, one)));
        Assert.IsTrue(size.Contains(new Rectangle<T>(zero, one, one, one), true));
        Assert.IsTrue(size.Contains(new Size<T>(one, one)));
        Assert.IsFalse(range.Contains(new HKW.HKWUtils.Drawing.Range<T>(zero, ten)));
        Assert.IsTrue(range.Contains(new HKW.HKWUtils.Drawing.Range<T>(zero, ten), true));
        Assert.IsFalse(new ObservableRange<T>(ten, zero).Contains(one));
        IReadOnlyPoint<T> first = point;
        IReadOnlyPoint<T> second = new Point<T>(one, two);
        Assert.IsTrue(first.ValueEquals(second));
        Assert.IsFalse(first == second);
        Assert.AreEqual(new Point<T>(two, T.CreateChecked(3)), point.Add(new Size<T>(one, one)));
        Assert.AreEqual(new Point<T>(zero, one), point.Subtract(new Size<T>(one, one)));
        Assert.AreEqual(
            new HKW.HKWUtils.Drawing.Range<T>(one, T.CreateChecked(11)),
            range.Add(new Size<T>(one, one))
        );
        var movedRange = new ObservableRange<T>(two, ten);
        Assert.AreEqual(
            new HKW.HKWUtils.Drawing.Range<T>(one, T.CreateChecked(9)),
            movedRange.Subtract(new Size<T>(one, one))
        );
        point.Offset(new Point<T>(one, one));
        Assert.AreEqual(two, point.X);
        point.SetValue(new Point<T>(one, two));
        size.SetValue(new Size<T>(two, two));
        range.SetValue(new HKW.HKWUtils.Drawing.Range<T>(one, two));
        rectangle.SetValue(new Rectangle<T>(one, two, two, two));
        Assert.AreEqual(one, range.Min);
        Assert.AreEqual(two, size.Width);
        Assert.AreEqual(one, rectangle.X);
        rectangle.SetValue(new Point<T>(zero, zero), new Size<T>(ten, ten));
        Assert.AreEqual(zero, rectangle.X);
        Assert.AreEqual(ten, rectangle.Width);
        Assert.IsTrue(
            new ObservableRectangle<T>(new Point<T>(zero, zero), new Size<T>(ten, ten)).Equals(
                rectangle
            )
        );
        if (T.IsNegative(-one))
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new ObservableSize<T>(-one, zero)
            );
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Height = -one);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Width = -one);
            point.X = -one;
            rectangle.X = -one;
            Assert.AreEqual(-one, point.X);
            Assert.AreEqual(-one, rectangle.X);
        }
    }

    [TestMethod]
    public void InvalidEndpointsAndNonFiniteValuesAreNotContained()
    {
        Assert.IsFalse(
            new ObservableRectangle<decimal>(decimal.MaxValue, 0, 1, 1).Contains(
                new Point<decimal>(0, 0)
            )
        );
        Assert.IsFalse(
            new ObservableRectangle<ulong>(ulong.MaxValue, 0, 1, 1).Contains(new Point<ulong>(0, 0))
        );
        Assert.IsFalse(
            new ObservableRectangle<double>(double.MaxValue, 0, double.MaxValue, 1).Contains(
                new Point<double>(0, 0)
            )
        );
        var rectangle = new ObservableRectangle<double>(0, 0, 10, 10);
        Assert.IsFalse(rectangle.Contains(new Rectangle<double>(double.PositiveInfinity, 0, 1, 1)));
        Assert.IsFalse(rectangle.Contains(new Point<double>(double.NaN, 0), true));
        Assert.IsFalse(
            new ObservableSize<double>(double.NaN, 10).Contains(new Point<double>(1, 1))
        );
        Assert.IsFalse(new ObservableRange<double>(0, double.PositiveInfinity).Contains(1));
        Assert.ThrowsExactly<OverflowException>(() =>
            _ = new ObservableRectangle<double>(double.MaxValue, 0, double.MaxValue, 1).RightTop
        );
    }

    [TestMethod]
    public void ParseMigrationUsesExactUnderlyingFieldsAndCulture()
    {
        var culture = CultureInfo.GetCultureInfo("de-DE");
        var point = new ObservablePoint<double>(Point<double>.Parse("1,5;2,5", ';', culture));
        var size = new ObservableSize<double>(Size<double>.Parse("1,5;2,5", ';', culture));
        var range = new ObservableRange<double>(
            HKW.HKWUtils.Drawing.Range<double>.Parse("1,5;2,5", ';', culture)
        );
        var rectangle = new ObservableRectangle<double>(
            Rectangle<double>.Parse("1,5;2,5;3,5;4,5", ';', culture)
        );
        Assert.AreEqual(1.5, point.X);
        Assert.AreEqual(2.5, size.Height);
        Assert.AreEqual(1.5, range.Min);
        Assert.AreEqual(4.5, rectangle.Height);
        Assert.ThrowsExactly<FormatException>(() =>
            new ObservablePoint<int>(Point<int>.Parse("1,2,3"))
        );
        Assert.ThrowsExactly<FormatException>(() => new ObservableSize<int>(Size<int>.Parse("1")));
        Assert.ThrowsExactly<FormatException>(() =>
            new ObservableRange<int>(HKW.HKWUtils.Drawing.Range<int>.Parse("1,"))
        );
        Assert.ThrowsExactly<FormatException>(() =>
            new ObservableRectangle<int>(Rectangle<int>.Parse("1,2,3,4,5"))
        );
    }

    [TestMethod]
    public void InvalidMultiPropertyDimensionsDoNotPublishPartialState()
    {
        var rectangle = new ObservableRectangle<int>(1, 2, 3, 4);
        var size = new ObservableSize<int>(3, 4);
        var calls = 0;
        rectangle.PropertyChanging += (_, _) => calls++;
        size.PropertyChanging += (_, _) => calls++;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.SetValue(5, 6, 7, -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.SetValue(5, -1));
        Assert.AreEqual(0, calls);
        Assert.AreEqual(1, rectangle.X);
        Assert.AreEqual(3, size.Width);
    }

    [TestMethod]
    public void ConcreteContainsAndMutationPathsAllocateNoBoxes()
    {
        var rectangle = new ObservableRectangle<int>(0, 0, 10, 10);
        var range = new ObservableRange<int>(0, 10);
        var size = new ObservableSize<int>(10, 10);
        var point = new ObservablePoint<int>();
        var p = new Point<int>(1, 1);
        var r = new HKW.HKWUtils.Drawing.Range<int>(1, 2);
        var s = new Size<int>(1, 1);
        var rect = new Rectangle<int>(1, 1, 2, 2);
        var result = true;
        for (var i = 0; i < 10000; i++)
        {
            result &=
                rectangle.Contains(p)
                && rectangle.Contains(rect)
                && range.Contains(r)
                && size.Contains(p)
                && size.Contains(s)
                && size.Contains(rect);
            point.SetValue(p);
        }
        var start = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10000; i++)
        {
            result &=
                rectangle.Contains(p)
                && rectangle.Contains(rect)
                && range.Contains(r)
                && size.Contains(p)
                && size.Contains(s)
                && size.Contains(rect);
            point.SetValue(p);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - start;
        Assert.IsTrue(result);
        Assert.AreEqual(0L, allocated);
    }
}
