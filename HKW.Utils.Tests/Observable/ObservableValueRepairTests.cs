using System.ComponentModel;
using HKW.HKWUtils;
using HKW.HKWUtils.Drawing;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableValueRepairTests
{
    [Flags]
    public enum Flags : int
    {
        None = 0,
        A = 1,
        AliasA = 1,
        B = 2,
        Both = 3,
    }

    [Flags]
    public enum OtherFlags : byte
    {
        None = 0,
        A = 1,
    }

    [TestMethod]
    public void ValueOperatorsHandleBothNullSides()
    {
        ObservableValue<string?>? missing = null;
        var empty = new ObservableValue<string?>(null);
        Assert.IsTrue(missing == null);
        Assert.IsFalse(missing == empty);
        Assert.IsFalse(empty == missing);
        Assert.IsTrue(empty != missing);
        Assert.IsTrue(empty == new ObservableValue<string?>(null));
        Assert.IsFalse(empty.Equals(null));
    }

    private static (
        INotifyPropertyChanging Changing,
        INotifyPropertyChanged Changed,
        Action Mutate,
        string[] Names
    ) Shape(string property)
    {
        return property switch
        {
            "Point.X" => Item(new ObservablePoint<int>(), x => x.X = 1, "X", "IsEmpty"),
            "Point.Y" => Item(new ObservablePoint<int>(), x => x.Y = 1, "Y", "IsEmpty"),
            "Range.Min" => Item(new ObservableRange<int>(), x => x.Min = 1, "Min", "IsEmpty"),
            "Range.Max" => Item(new ObservableRange<int>(), x => x.Max = 1, "Max", "IsEmpty"),
            "Size.Width" => Item(new ObservableSize<int>(), x => x.Width = 1, "Width", "IsEmpty"),
            "Size.Height" => Item(
                new ObservableSize<int>(),
                x => x.Height = 1,
                "Height",
                "IsEmpty"
            ),
            "Rectangle.X" => Item(
                new ObservableRectangle<int>(),
                x => x.X = 1,
                "X",
                "IsEmpty",
                "Left",
                "Right",
                "LeftTop",
                "RightTop",
                "LeftBottom",
                "RightBottom"
            ),
            "Rectangle.Y" => Item(
                new ObservableRectangle<int>(),
                x => x.Y = 1,
                "Y",
                "IsEmpty",
                "Top",
                "Bottom",
                "LeftTop",
                "RightTop",
                "LeftBottom",
                "RightBottom"
            ),
            "Rectangle.Width" => Item(
                new ObservableRectangle<int>(),
                x => x.Width = 1,
                "Width",
                "IsEmpty",
                "Right",
                "RightTop",
                "RightBottom"
            ),
            "Rectangle.Height" => Item(
                new ObservableRectangle<int>(),
                x => x.Height = 1,
                "Height",
                "IsEmpty",
                "Bottom",
                "LeftBottom",
                "RightBottom"
            ),
            "Enum.Value" => Item(
                new ObservableEnum<Flags>(),
                x => x.Value = Flags.A,
                "Value",
                "Info"
            ),
            _ => Item(new ObservableValue<int>(), x => x.Value = 1, "Value"),
        };
    }

    private static (INotifyPropertyChanging, INotifyPropertyChanged, Action, string[]) Item<T>(
        T item,
        Action<T> action,
        params string[] names
    )
        where T : INotifyPropertyChanging, INotifyPropertyChanged =>
        (item, item, () => action(item), names);

    [TestMethod]
    [DataRow("Point.X")]
    [DataRow("Point.Y")]
    [DataRow("Range.Min")]
    [DataRow("Range.Max")]
    [DataRow("Size.Width")]
    [DataRow("Size.Height")]
    [DataRow("Rectangle.X")]
    [DataRow("Rectangle.Y")]
    [DataRow("Rectangle.Width")]
    [DataRow("Rectangle.Height")]
    [DataRow("Enum.Value")]
    [DataRow("Value.Value")]
    public void SetterUsesOneSubscriberSnapshot(string property)
    {
        var item = Shape(property);
        var before = new List<string>();
        var after = new List<string>();
        var late = 0;
        PropertyChangingEventHandler? changing = null;
        PropertyChangedEventHandler changed = (_, e) => after.Add(e.PropertyName!);
        changing = (_, e) =>
        {
            before.Add(e.PropertyName!);
            item.Changing.PropertyChanging -= changing;
            item.Changed.PropertyChanged -= changed;
            item.Changed.PropertyChanged += (_, _) => late++;
        };
        item.Changing.PropertyChanging += changing;
        item.Changed.PropertyChanged += changed;
        item.Mutate();
        CollectionAssert.AreEqual(item.Names, before);
        CollectionAssert.AreEqual(item.Names, after);
        Assert.AreEqual(0, late);
        item.Mutate();
        Assert.AreEqual(0, late);
    }

    [TestMethod]
    public void NegativeDimensionsAreRejectedBeforeNotification()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ObservableSize<int>(-1, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new ObservableRectangle<int>(-2, -3, 0, -1)
        );
        var size = new ObservableSize<int>(2, 3);
        var rectangle = new ObservableRectangle<int>(-2, -3, 2, 3);
        var calls = 0;
        size.PropertyChanging += (_, _) => calls++;
        rectangle.PropertyChanging += (_, _) => calls++;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Width = -1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => size.Height = -1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Width = -1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Height = -1);
        Assert.AreEqual(0, calls);
        Assert.AreEqual(2, size.Width);
        Assert.AreEqual(3, rectangle.Height);
        Assert.AreEqual(-2, rectangle.X);
    }

    [TestMethod]
    public void RectangleEndpointsRejectOverflow()
    {
        var rectangle = new ObservableRectangle<int>(int.MaxValue, int.MaxValue, 1, 1);
        Assert.ThrowsExactly<OverflowException>(() => _ = rectangle.Right);
        Assert.ThrowsExactly<OverflowException>(() => _ = rectangle.Bottom);
        Assert.IsFalse(rectangle.Contains(new Point<int>(0, 0)));
    }

    [TestMethod]
    public void AllGeometryEqualsUseNaNValueSemantics()
    {
        var point = new ObservablePoint<double>(double.NaN, 1);
        var range = new ObservableRange<double>(double.NaN, 1);
        var size = new ObservableSize<double>(double.NaN, 1);
        var rectangle = new ObservableRectangle<double>(double.NaN, 1, 2, 3);
        Assert.IsTrue(point.Equals(new Point<double>(double.NaN, 1)));
        Assert.IsTrue(range.Equals(new HKW.HKWUtils.Drawing.Range<double>(double.NaN, 1)));
        Assert.IsTrue(size.Equals((object)new Size<double>(double.NaN, 1)));
        Assert.IsTrue(rectangle.Equals(new Rectangle<double>(double.NaN, 1, 2, 3)));
        Assert.AreEqual(new Size<double>(double.NaN, 1).GetHashCode(), size.GetHashCode());
        var calls = 0;
        point.PropertyChanged += (_, _) => calls++;
        range.PropertyChanged += (_, _) => calls++;
        size.PropertyChanged += (_, _) => calls++;
        rectangle.PropertyChanged += (_, _) => calls++;
        point.X = range.Min = size.Width = rectangle.X = double.NaN;
        Assert.AreEqual(0, calls);
    }

    [TestMethod]
    public void ClonePreservesInstanceStrategiesButNotSubscriptionsOrCommands()
    {
        var source = new ObservableEnum<Flags>(Flags.A, (_, _) => Flags.Both, (_, _) => Flags.B);
        var calls = 0;
        source.PropertyChanged += (_, _) => calls++;
        var clone = (ObservableEnum<Flags>)source.Clone();
        Assert.AreSame(source.AddFlagFunc, clone.AddFlagFunc);
        Assert.AreSame(source.RemoveFlagFunc, clone.RemoveFlagFunc);
        Assert.AreNotSame(source.AddFlagCommand, clone.AddFlagCommand);
        clone.AddFlag(Flags.None);
        Assert.AreEqual(Flags.Both, clone.Value);
        clone.RemoveFlag(Flags.None);
        Assert.AreEqual(Flags.B, clone.Value);
        Assert.AreEqual(Flags.A, source.Value);
        Assert.AreEqual(0, calls);
    }

    [TestMethod]
    public void FactoryConcurrentFirstUseAndNullContract()
    {
        Parallel.For(
            0,
            128,
            i =>
            {
                Enum input = i % 2 == 0 ? Flags.A : OtherFlags.A;
                Assert.AreEqual(input, ObservableEnum.Create(input).Value);
            }
        );
        Assert.ThrowsExactly<ArgumentNullException>(() => ObservableEnum.Create(null!));
    }

    [TestMethod]
    public void FlagsNoneAliasesAndUnknownBitsPreserveSourceValue()
    {
        var value = new ObservableEnum<Flags>((Flags)8);
        value.AddFlag(Flags.None);
        Assert.AreEqual((Flags)8, value.Value);
        value.AddFlag(Flags.AliasA);
        Assert.AreEqual((Flags)9, value.Value);
        Assert.AreEqual(value.Value, value.Info.Value);
        value.RemoveFlag(Flags.A);
        Assert.AreEqual((Flags)8, value.Value);
        value.RemoveFlag(Flags.None);
        Assert.AreEqual((Flags)8, value.Value);
        Assert.AreSame(Flags.A.GetInfo(), Flags.AliasA.GetInfo());
    }

    [TestMethod]
    public void ConcreteGeometryEqualityAllocatesNoBoxes()
    {
        var point = new ObservablePoint<double>(1, 2);
        var range = new ObservableRange<double>(1, 2);
        var size = new ObservableSize<double>(1, 2);
        var rectangle = new ObservableRectangle<double>(1, 2, 3, 4);
        var p = new Point<double>(1, 2);
        var r = new HKW.HKWUtils.Drawing.Range<double>(1, 2);
        var s = new Size<double>(1, 2);
        var rect = new Rectangle<double>(1, 2, 3, 4);
        var equal = true;
        for (var i = 0; i < 10000; i++)
            equal &= point.Equals(p) && range.Equals(r) && size.Equals(s) && rectangle.Equals(rect);
        var start = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10000; i++)
            equal &= point.Equals(p) && range.Equals(r) && size.Equals(s) && rectangle.Equals(rect);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - start;
        Assert.IsTrue(equal);
        Assert.AreEqual(0L, allocated);
    }

    [TestMethod]
    public void CloneWithDefaultStrategiesRemainsDefaultBound()
    {
        var oldAdd = ObservableEnum<OtherFlags>.DefaultAddFlagFunc;
        var oldRemove = ObservableEnum<OtherFlags>.DefaultRemoveFlagFunc;
        try
        {
            var source = new ObservableEnum<OtherFlags>();
            var clone = (ObservableEnum<OtherFlags>)source.Clone();
            ObservableEnum<OtherFlags>.DefaultAddFlagFunc = (_, _) => OtherFlags.A;
            ObservableEnum<OtherFlags>.DefaultRemoveFlagFunc = (_, _) => OtherFlags.None;
            clone.AddFlag(OtherFlags.None);
            Assert.AreEqual(OtherFlags.A, clone.Value);
            clone.RemoveFlag(OtherFlags.None);
            Assert.AreEqual(OtherFlags.None, clone.Value);
        }
        finally
        {
            ObservableEnum<OtherFlags>.DefaultAddFlagFunc = oldAdd;
            ObservableEnum<OtherFlags>.DefaultRemoveFlagFunc = oldRemove;
        }
    }

    [TestMethod]
    [DataRow("Point.X")]
    [DataRow("Point.Y")]
    [DataRow("Range.Min")]
    [DataRow("Range.Max")]
    [DataRow("Size.Width")]
    [DataRow("Size.Height")]
    [DataRow("Rectangle.X")]
    [DataRow("Rectangle.Y")]
    [DataRow("Rectangle.Width")]
    [DataRow("Rectangle.Height")]
    [DataRow("Enum.Value")]
    [DataRow("Value.Value")]
    public void ChangedSelfRemovalAndAdditionUseSnapshot(string property)
    {
        var item = Shape(property);
        var seen = new List<string>();
        var late = 0;
        PropertyChangedEventHandler? handler = null;
        handler = (_, e) =>
        {
            seen.Add(e.PropertyName!);
            item.Changed.PropertyChanged -= handler;
            item.Changed.PropertyChanged += (_, _) => late++;
        };
        item.Changed.PropertyChanged += handler;
        item.Mutate();
        CollectionAssert.AreEqual(item.Names, seen);
        Assert.AreEqual(0, late);
    }

    [TestMethod]
    public void CallbackExceptionsKeepDefinedCommitBoundary()
    {
        var point = new ObservablePoint<int>();
        PropertyChangingEventHandler reject = (_, _) =>
            throw new InvalidOperationException("Rejected.");
        point.PropertyChanging += reject;
        Assert.ThrowsExactly<InvalidOperationException>(() => point.X = 1);
        Assert.AreEqual(0, point.X);
        point.PropertyChanging -= reject;
        point.PropertyChanged += (_, _) => throw new InvalidOperationException("Committed.");
        Assert.ThrowsExactly<InvalidOperationException>(() => point.X = 1);
        Assert.AreEqual(1, point.X);
    }

    [TestMethod]
    public void GeometryCloneCopiesValuesWithoutSubscriptions()
    {
        var point = new ObservablePoint<int>(1, 2);
        var range = new ObservableRange<int>(1, 2);
        var size = new ObservableSize<int>(1, 2);
        var rectangle = new ObservableRectangle<int>(1, 2, 3, 4);
        var calls = 0;
        point.PropertyChanged += (_, _) => calls++;
        range.PropertyChanged += (_, _) => calls++;
        size.PropertyChanged += (_, _) => calls++;
        rectangle.PropertyChanged += (_, _) => calls++;
        var p = point.Clone();
        var r = range.Clone();
        var s = size.Clone();
        var rect = rectangle.Clone();
        Assert.IsTrue(point.Equals(p));
        Assert.IsTrue(range.Equals(r));
        Assert.IsTrue(size.Equals(s));
        Assert.IsTrue(rectangle.Equals(rect));
        p.X = r.Min = s.Width = rect.X = 5;
        Assert.AreEqual(0, calls);
        Assert.AreEqual(1, point.X);
        Assert.AreEqual(1, range.Min);
        Assert.AreEqual(1, size.Width);
        Assert.AreEqual(1, rectangle.X);
    }

    [Flags]
    public enum SignedByteFlags : sbyte
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum ByteFlags : byte
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum ShortFlags : short
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum UShortFlags : ushort
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum IntFlags : int
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum UIntFlags : uint
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum LongFlags : long
    {
        None = 0,
        A = 1,
    }

    [Flags]
    public enum ULongFlags : ulong
    {
        None = 0,
        A = 1,
    }

    [TestMethod]
    public void AllEnumWidthsPreserveUnknownBitsAndNone()
    {
        CheckFlags<SignedByteFlags>();
        CheckFlags<ByteFlags>();
        CheckFlags<ShortFlags>();
        CheckFlags<UShortFlags>();
        CheckFlags<IntFlags>();
        CheckFlags<UIntFlags>();
        CheckFlags<LongFlags>();
        CheckFlags<ULongFlags>();
    }

    private static void CheckFlags<TEnum>()
        where TEnum : struct, Enum
    {
        var all = (TEnum)Enum.ToObject(typeof(TEnum), -1L);
        var one = (TEnum)Enum.ToObject(typeof(TEnum), 1);
        var value = new ObservableEnum<TEnum>(all);
        var calls = 0;
        value.PropertyChanged += (_, _) => calls++;
        value.AddFlag(default);
        value.RemoveFlag(default);
        Assert.AreEqual(all, value.Value);
        Assert.AreEqual(0, calls);
        value.RemoveFlag(one);
        Assert.AreEqual(all.RemoveFlag(one), value.Value);
        Assert.AreEqual(value.Value, value.Info.Value);
        Assert.AreEqual(value.Value.ToString(), value.Info.DisplayName);
        value.AddFlag(one);
        Assert.AreEqual(all, value.Value);
        Assert.AreEqual(4, calls);
    }

    [TestMethod]
    public void EnumSettersAllocateNoBoxesWithoutObservers()
    {
        var value = new ObservableEnum<Flags>();
        for (var i = 0; i < 10000; i++)
            value.Value = i % 2 == 0 ? Flags.A : Flags.B;
        var start = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10000; i++)
            value.Value = i % 2 == 0 ? Flags.A : Flags.B;
        Assert.AreEqual(0L, GC.GetAllocatedBytesForCurrentThread() - start);
        Assert.AreEqual(Flags.B, value.Value);
    }

    [TestMethod]
    public void SubscriptionChangesStartWithNextMutation()
    {
        var point = new ObservablePoint<int>();
        var oldCalls = 0;
        var newCalls = 0;
        PropertyChangedEventHandler original = (_, _) => oldCalls++;
        PropertyChangingEventHandler? before = null;
        before = (_, _) =>
        {
            point.PropertyChanging -= before;
            point.PropertyChanged -= original;
            point.PropertyChanged += (_, _) => newCalls++;
        };
        point.PropertyChanging += before;
        point.PropertyChanged += original;
        point.X = 1;
        Assert.AreEqual(2, oldCalls);
        Assert.AreEqual(0, newCalls);
        point.X = 2;
        Assert.AreEqual(2, oldCalls);
        Assert.AreEqual(4, newCalls);
    }

    [TestMethod]
    [DataRow("Point.X")]
    [DataRow("Point.Y")]
    [DataRow("Range.Min")]
    [DataRow("Range.Max")]
    [DataRow("Size.Width")]
    [DataRow("Size.Height")]
    [DataRow("Rectangle.X")]
    [DataRow("Rectangle.Y")]
    [DataRow("Rectangle.Width")]
    [DataRow("Rectangle.Height")]
    [DataRow("Enum.Value")]
    [DataRow("Value.Value")]
    public void SettersRejectReentrantWritesAndRecover(string property)
    {
        var item = Shape(property);
        var changingCalls = 0;
        var changedCalls = 0;
        var checking = false;
        PropertyChangingEventHandler changing = (_, _) =>
        {
            if (checking)
                return;
            changingCalls++;
            checking = true;
            try
            {
                Assert.ThrowsExactly<InvalidOperationException>(item.Mutate);
            }
            finally
            {
                checking = false;
            }
        };
        PropertyChangedEventHandler changed = (_, _) =>
        {
            if (checking)
                return;
            changedCalls++;
            checking = true;
            try
            {
                Assert.ThrowsExactly<InvalidOperationException>(item.Mutate);
            }
            finally
            {
                checking = false;
            }
        };
        item.Changing.PropertyChanging += changing;
        item.Changed.PropertyChanged += changed;
        item.Mutate();
        Assert.AreEqual(item.Names.Length, changingCalls);
        Assert.AreEqual(item.Names.Length, changedCalls);
        item.Mutate();
        Assert.AreEqual(item.Names.Length, changingCalls);
        Assert.AreEqual(item.Names.Length, changedCalls);

        var rejected = Shape(property);
        PropertyChangingEventHandler reject = (_, _) =>
            throw new InvalidOperationException("Rejected.");
        rejected.Changing.PropertyChanging += reject;
        Assert.ThrowsExactly<InvalidOperationException>(rejected.Mutate);
        rejected.Changing.PropertyChanging -= reject;
        var committed = 0;
        PropertyChangedEventHandler failAfterCommit = (_, _) =>
        {
            committed++;
            throw new InvalidOperationException("Committed.");
        };
        rejected.Changed.PropertyChanged += failAfterCommit;
        Assert.ThrowsExactly<InvalidOperationException>(rejected.Mutate);
        Assert.AreEqual(1, committed);
        rejected.Changed.PropertyChanged -= failAfterCommit;
        rejected.Mutate();
        var recovered = 0;
        rejected.Changed.PropertyChanged += (_, _) => recovered++;
        var propertyName = property[(property.IndexOf('.') + 1)..];
        rejected
            .Changing.GetType()
            .GetProperty(propertyName)!
            .SetValue(rejected.Changing, property == "Enum.Value" ? (object)Flags.B : 2);
        Assert.AreEqual(item.Names.Length, recovered);
    }

    [TestMethod]
    [DataRow("Point.X")]
    [DataRow("Point.Y")]
    [DataRow("Range.Min")]
    [DataRow("Range.Max")]
    [DataRow("Size.Width")]
    [DataRow("Size.Height")]
    [DataRow("Rectangle.X")]
    [DataRow("Rectangle.Y")]
    [DataRow("Rectangle.Width")]
    [DataRow("Rectangle.Height")]
    public void GeometryRejectsOtherPropertyWritesDuringBothNotifications(string property)
    {
        var item = Shape(property);
        Action[] writeOther = item.Changing switch
        {
            ObservablePoint<int> p => [() => p.X = p.X, () => p.Y = p.Y],
            ObservableRange<int> r => [() => r.Min = r.Min, () => r.Max = r.Max],
            ObservableSize<int> s => [() => s.Width = s.Width, () => s.Height = s.Height],
            ObservableRectangle<int> r =>
            [
                () => r.X = r.X,
                () => r.Y = r.Y,
                () => r.Width = r.Width,
                () => r.Height = r.Height,
            ],
            _ => throw new InvalidOperationException("Unexpected shape."),
        };
        var other = new ObservableValue<int>();
        item.Changing.PropertyChanging += (_, _) =>
        {
            foreach (var write in writeOther)
                Assert.ThrowsExactly<InvalidOperationException>(write);
            other.Value++;
        };
        item.Changed.PropertyChanged += (_, _) =>
        {
            foreach (var write in writeOther)
                Assert.ThrowsExactly<InvalidOperationException>(write);
            other.Value++;
        };
        item.Mutate();
        Assert.AreEqual(item.Names.Length * 2, other.Value);
    }

    [TestMethod]
    public void ValidationAndNoOpReleaseMutationGuard()
    {
        var rectangle = new ObservableRectangle<int>();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => rectangle.Width = -1);
        rectangle.Width = 0;
        rectangle.PropertyChanging += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => rectangle.Width = -1);
        rectangle.PropertyChanged += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => rectangle.Height = -1);
        rectangle.SetValue(1, 2, 3, 4);
        Assert.AreEqual(4, rectangle.Height);
        var point = new ObservablePoint<int>();
        point.X = 0;
        point.SetValue(new Point<int>(1, 2));
        point.Offset(new Point<int>(1, 1));
        Assert.AreEqual(2, point.X);
        Assert.AreEqual(3, point.Y);
    }

    private sealed class CallbackEquality : IEquatable<CallbackEquality>
    {
        public Action? Comparing { get; set; }

        public bool Equals(CallbackEquality? other)
        {
            Comparing?.Invoke();
            return ReferenceEquals(this, other);
        }
    }

    [TestMethod]
    public void SubscriptionSnapshotAndGuardPrecedeEqualityValidation()
    {
        var initial = new CallbackEquality();
        var value = new ObservableValue<CallbackEquality>(initial);
        var next = new CallbackEquality();
        var calls = 0;
        PropertyChangedEventHandler changed = (_, _) => calls++;
        value.PropertyChanged += changed;
        initial.Comparing = () =>
        {
            initial.Comparing = null;
            value.PropertyChanged -= changed;
            Assert.ThrowsExactly<InvalidOperationException>(() => value.Value = initial);
        };
        value.Value = next;
        Assert.AreEqual(1, calls);
        var throwing = new CallbackEquality();
        value.Value = throwing;
        Assert.AreEqual(1, calls);
        throwing.Comparing = () => throw new InvalidOperationException("Equality failed.");
        Assert.ThrowsExactly<InvalidOperationException>(() => value.Value = next);
        Assert.AreSame(throwing, value.Value);
        throwing.Comparing = null;
        value.Value = next;
        Assert.AreSame(next, value.Value);
    }

    [TestMethod]
    public void EnumCompositeOperationsRejectNestedWritesBeforeStrategiesAndCasts()
    {
        var strategies = 0;
        var value = new ObservableEnum<Flags>(
            Flags.None,
            (v, f) =>
            {
                strategies++;
                return v | f;
            },
            (v, f) =>
            {
                strategies++;
                return v & ~f;
            }
        );
        void CheckNestedWrites()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => value.AddFlag(Flags.None));
            Assert.ThrowsExactly<InvalidOperationException>(() => value.RemoveFlag(Flags.None));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                value.AddFlagFunc = value.AddFlagFunc
            );
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                value.RemoveFlagFunc = value.RemoveFlagFunc
            );
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ((IObservableEnum)value).Value = OtherFlags.A
            );
        }
        value.PropertyChanging += (_, _) => CheckNestedWrites();
        value.PropertyChanged += (_, _) => CheckNestedWrites();
        value.AddFlag(Flags.A);
        Assert.AreEqual(Flags.A, value.Value);
        value.RemoveFlag(Flags.A);
        Assert.AreEqual(Flags.None, value.Value);
        Assert.AreEqual(2, strategies);
        Assert.ThrowsExactly<InvalidCastException>(() =>
            ((IObservableEnum)value).Value = OtherFlags.A
        );
        ((IObservableEnum)value).Value = Flags.B;
        Assert.AreEqual(Flags.B, value.Value);
    }

    [TestMethod]
    public void EnumStrategySnapshotAndFailureReleaseScope()
    {
        var value = new ObservableEnum<Flags>();
        var calls = 0;
        PropertyChangedEventHandler changed = (_, _) => calls++;
        value.PropertyChanged += changed;
        value.AddFlagFunc = (_, _) =>
        {
            value.PropertyChanged -= changed;
            Assert.ThrowsExactly<InvalidOperationException>(() => value.Value = value.Value);
            return Flags.A;
        };
        value.AddFlag(Flags.None);
        Assert.AreEqual(2, calls);
        value.AddFlagFunc = (_, _) => throw new InvalidOperationException("Strategy failed.");
        Assert.ThrowsExactly<InvalidOperationException>(() => value.AddFlag(Flags.B));
        Assert.AreEqual(Flags.A, value.Value);
        value.AddFlagFunc = (v, f) => v | f;
        value.AddFlag(Flags.B);
        Assert.AreEqual(Flags.Both, value.Value);
    }

    [TestMethod]
    public void CommandRetainsCallerControlledCanExecuteAndCastSemantics()
    {
        var result = 0;
        var command = new SimpleCommand<int>(value => result = value, () => false);
        Assert.IsFalse(command.CanExecute("ignored"));
        command.Execute(42);
        Assert.AreEqual(42, result);
        Assert.ThrowsExactly<InvalidCastException>(() => command.Execute("42"));
        Assert.ThrowsExactly<NullReferenceException>(() => command.Execute(null));
        var notified = 0;
        command.CanExecuteChanged += (_, _) => notified++;
        command.NotifyCanExecuteChanged();
        Assert.AreEqual(1, notified);
        new SimpleCommand<int?>(value => Assert.IsNull(value)).Execute(null);
        new SimpleCommand<string?>(value => Assert.IsNull(value)).Execute(null);
    }
}
