using System.ComponentModel;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableStateGroupContractTests
{
    [TestMethod]
    public void ChangedSeesFinalStateAndSynchronizedMembers()
    {
        var member = new Member(1);
        var group = new Group(member);
        var states = new List<int>();
        group.PropertyChanging += (_, _) => Assert.AreEqual(0, group.State);
        group.PropertyChanged += (_, _) =>
        {
            states.Add(group.State);
            Assert.AreEqual(7, member.Value);
        };
        group.State = 99;
        Assert.AreEqual(7, group.State);
        CollectionAssert.AreEqual(new[] { 7 }, states);
    }

    [TestMethod]
    public void RemovingEqualMemberUsesStoredInstanceAndDetachesIt()
    {
        var stored = new Member(1);
        var group = new Group(stored);
        Assert.IsTrue(group.Remove(new Member(1)));
        Assert.AreSame(stored, group.LastRemoved);
        Assert.AreEqual(0, stored.SubscriberCount);
        stored.Change(42);
        Assert.AreEqual(0, group.MemberEvents);
        Assert.IsFalse(group.Remove(stored));
    }

    [TestMethod]
    public void ClearCallbackFailureKeepsMembersAndSubscriptions()
    {
        var member = new Member(1);
        var group = new Group(member) { ThrowClearing = true };
        Assert.ThrowsExactly<InvalidOperationException>(() => group.Clear());
        Assert.AreEqual(1, group.Count);
        Assert.AreEqual(1, member.SubscriberCount);
        member.Change(4);
        Assert.AreEqual(1, group.MemberEvents);
        Assert.AreEqual(4, group.State);
        group.ThrowClearing = false;
        group.Clear();
        Assert.AreEqual(0, group.Count);
        Assert.AreEqual(0, member.SubscriberCount);
    }

    [TestMethod]
    [DataRow("State")]
    [DataRow("Add")]
    [DataRow("Remove")]
    [DataRow("Clear")]
    public void ChangingFailureDoesNotCommit(string operation)
    {
        var stored = new Member(1);
        var added = new Member(2);
        var group = new Group(stored);
        PropertyChangingEventHandler throwing = (_, _) => throw new InvalidOperationException();
        group.PropertyChanging += throwing;
        Assert.ThrowsExactly<InvalidOperationException>(() => Write(group, operation, added));
        Assert.AreEqual(1, group.Count);
        Assert.IsTrue(group.Contains(stored));
        Assert.AreEqual(1, stored.SubscriberCount);
        Assert.AreEqual(0, added.SubscriberCount);
        Assert.AreEqual(0, group.State);
        Assert.AreEqual(0, stored.Value);
        group.PropertyChanging -= throwing;
        group.State = 3;
        Assert.AreEqual(3, group.State);
    }

    [TestMethod]
    [DataRow("State")]
    [DataRow("Add")]
    [DataRow("Remove")]
    [DataRow("Clear")]
    public void ChangedFailurePreservesCommittedMembershipAndState(string operation)
    {
        var stored = new Member(1);
        var added = new Member(2);
        var group = new Group(stored);
        PropertyChangedEventHandler throwing = (_, _) => throw new InvalidOperationException();
        group.PropertyChanged += throwing;
        Assert.ThrowsExactly<InvalidOperationException>(() => Write(group, operation, added));
        var expectedCount = operation switch
        {
            "Add" => 2,
            "State" => 1,
            _ => 0,
        };
        Assert.AreEqual(expectedCount, group.Count);
        Assert.AreEqual(operation is "Clear" or "Remove" ? 0 : 1, stored.SubscriberCount);
        Assert.AreEqual(operation == "Add" ? 1 : 0, added.SubscriberCount);
        Assert.AreEqual(
            operation == "State" ? 3
                : operation == "Add" ? 2
                : -1,
            group.State
        );
        group.PropertyChanged -= throwing;
        group.State = 5;
        Assert.AreEqual(5, group.State);
    }

    [TestMethod]
    public void EntireOperationUsesSubscriptionSnapshot()
    {
        var group = new Group(new Member(1));
        var originalCalls = 0;
        var lateCalls = 0;
        PropertyChangedEventHandler original = (_, _) => originalCalls++;
        PropertyChangedEventHandler late = (_, _) => lateCalls++;
        group.PropertyChanged += original;
        group.PropertyChanging += (_, _) =>
        {
            group.PropertyChanged -= original;
            group.PropertyChanged -= late;
            group.PropertyChanged += late;
        };
        group.State = 3;
        Assert.AreEqual(1, originalCalls);
        Assert.AreEqual(0, lateCalls);
        group.State = 4;
        Assert.AreEqual(1, originalCalls);
        Assert.AreEqual(1, lateCalls);
    }

    [TestMethod]
    public void EveryWriteRejectsReentrancyIncludingNoOps()
    {
        var member = new Member(1);
        var group = new Group(member);
        void Check()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => group.State = group.State);
            Assert.ThrowsExactly<InvalidOperationException>(() => group.Add(member));
            Assert.ThrowsExactly<InvalidOperationException>(() => group.Remove(new Member(999)));
            Assert.ThrowsExactly<InvalidOperationException>(() => group.Clear());
            Assert.AreEqual(1, group.Count);
        }
        group.PropertyChanging += (_, _) => Check();
        group.PropertyChanged += (_, _) => Check();
        member.PropertyChanged += (_, _) => Check();
        group.State = 3;
        Assert.AreEqual(3, group.State);
    }

    [TestMethod]
    public void MemberHooksFailWithoutLeavingSubscriptionHalfState()
    {
        var stored = new Member(1);
        var added = new Member(2);
        var group = new Group(stored) { ThrowMemberHook = true };
        Assert.ThrowsExactly<InvalidOperationException>(() => group.Add(added));
        Assert.IsFalse(group.Contains(added));
        Assert.AreEqual(0, added.SubscriberCount);
        Assert.ThrowsExactly<InvalidOperationException>(() => group.Remove(stored));
        Assert.IsTrue(group.Contains(stored));
        Assert.AreEqual(1, stored.SubscriberCount);
        group.ThrowMemberHook = false;
        stored.Change(4);
        Assert.AreEqual(4, group.State);
    }

    [TestMethod]
    public void DuplicateWritesAndEmptyClearHaveNoEvents()
    {
        var member = new Member(1);
        var group = new Group(member);
        var calls = 0;
        group.PropertyChanging += (_, _) => calls++;
        group.PropertyChanged += (_, _) => calls++;
        group.Add(new Member(1));
        Assert.IsFalse(group.Remove(new Member(2)));
        group.State = 0;
        Assert.AreEqual(0, calls);
        Assert.AreEqual(1, member.SubscriberCount);
        group.Clear();
        calls = 0;
        group.Clear();
        Assert.AreEqual(0, calls);
    }

    [TestMethod]
    public void StateCallbackFailureReleasesGuardWithoutCommittingRequestedState()
    {
        var member = new Member(1);
        var group = new Group(member);
        PropertyChangedEventHandler throwing = (_, _) => throw new InvalidOperationException();
        member.PropertyChanged += throwing;
        Assert.ThrowsExactly<InvalidOperationException>(() => group.State = 3);
        Assert.AreEqual(0, group.State);
        Assert.AreEqual(3, member.Value);
        member.PropertyChanged -= throwing;
        group.State = 4;
        Assert.AreEqual(4, group.State);
    }

    [TestMethod]
    public void CorrectedRequestDoesNotPublishTransientState()
    {
        var group = new Group(new Member(1)) { State = 7 };
        var changing = 0;
        var changed = 0;
        group.PropertyChanging += (_, _) => changing++;
        group.PropertyChanged += (_, _) => changed++;
        group.State = 99;
        Assert.AreEqual(7, group.State);
        Assert.AreEqual(1, changing);
        Assert.AreEqual(0, changed);
    }

    [TestMethod]
    public void NullMembersAreRejectedBeforeSubscriptionsAreInstalled()
    {
        var member = new Member(1);
        Assert.ThrowsExactly<ArgumentNullException>(() => new Group(member, null!));
        Assert.AreEqual(0, member.SubscriberCount);
        var group = new Group(member);
        Assert.ThrowsExactly<ArgumentNullException>(() => group.Add(null!));
        Assert.AreEqual(1, group.Count);
        group.State = 3;
        Assert.AreEqual(3, group.State);
    }

    private static void Write(Group group, string operation, Member added)
    {
        switch (operation)
        {
            case "State":
                group.State = 3;
                break;
            case "Add":
                group.Add(added);
                break;
            case "Remove":
                group.Remove(new Member(1));
                break;
            case "Clear":
                group.Clear();
                break;
        }
    }

    private sealed class Group(params Member[] members)
        : ObservableStateGroup<int, Member>(members, true)
    {
        public bool ThrowClearing { get; set; }
        public bool ThrowMemberHook { get; set; }
        public Member? LastRemoved { get; private set; }
        public int MemberEvents { get; private set; }

        protected override int InitializeState(ICollection<Member> members) => 0;

        protected override int StateChanged(int state, ICollection<Member> members)
        {
            var result = state == 99 ? 7 : state;
            foreach (var member in members)
                member.Change(result);
            return result;
        }

        protected override int MemberAdded(Member member)
        {
            if (ThrowMemberHook)
                throw new InvalidOperationException();
            return Count;
        }

        protected override int MemberRemoved(Member member)
        {
            if (ThrowMemberHook)
                throw new InvalidOperationException();
            LastRemoved = member;
            return -1;
        }

        protected override int MemberClearing(ICollection<Member> members)
        {
            if (ThrowClearing)
                throw new InvalidOperationException();
            return -1;
        }

        protected override int MemberPropertyChanged(Member member, PropertyChangedEventArgs e)
        {
            MemberEvents++;
            return member.Value;
        }
    }

    private sealed class Member(int id) : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler? _changed;
        public int Value { get; private set; }
        public int SubscriberCount => _changed?.GetInvocationList().Length ?? 0;
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        public void Change(int value)
        {
            Value = value;
            _changed?.Invoke(this, new(nameof(Value)));
        }

        public override bool Equals(object? obj) => obj is Member other && id == other.Id;

        private int Id => id;

        public override int GetHashCode() => id;
    }
}
