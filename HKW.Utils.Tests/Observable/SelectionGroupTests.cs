using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
[ReferenceType(typeof(ReactiveObject))]
public class SelectionGroupTests
{
    [TestMethod]
    public void InitializeAllMemberTrue()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember() { IsSelected = true })
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        Assert.IsTrue(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(3, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void InitializeAllMemberFalse()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember())
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        Assert.IsTrue(group.Leader.Value is false);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(0, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void SetLeaderTrue()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember())
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        leader.IsSelected = true;
        Assert.IsTrue(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(group.Members.Count, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void SetLeaderFalse()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember() { IsSelected = true })
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        leader.IsSelected = false;
        Assert.IsTrue(group.Leader.Value is false);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(0, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void AddMemberWhenAllFalse()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember())
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Add(new());
        Assert.IsTrue(group.Leader.Value is false);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(0, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members.Add(new() { IsSelected = true });
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.ContainsSingle(x => x.IsSelected, members);
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void AddMemberWhenAllTrue()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember() { IsSelected = true })
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Add(new());
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(3, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members.Add(new() { IsSelected = true });
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(4, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void RemoveMember()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember() { IsSelected = true })
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Remove(members.Last());
        Assert.IsTrue(group.Leader.Value is true);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(2, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members.Remove(members.Last());
        Assert.IsTrue(group.Leader.Value is true);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.ContainsSingle(x => x.IsSelected, members);
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members.Remove(members.Last());
        Assert.IsTrue(group.Leader.Value is false);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(0, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void ChangeMemberWhenAllTrue()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember() { IsSelected = true })
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members[0].IsSelected = false;
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(2, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members[1].IsSelected = false;
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.ContainsSingle(x => x.IsSelected, members);
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members[2].IsSelected = false;
        Assert.IsTrue(group.Leader.Value is false);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(0, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }

    [TestMethod]
    public void ChangeMemberWhenAllFalse()
    {
        var leader = new ObservableSelectionGroupLeader();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new ObservableSelectionGroupMember())
            .ToObservableList();
        var group = new ObservableSelectionGroup<
            ObservableSelectionGroupLeader,
            ObservableSelectionGroupMember,
            ObservableList<ObservableSelectionGroupMember>
        >(
            new(
                leader,
                nameof(ObservableSelectionGroupLeader.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(ObservableSelectionGroupMember.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members[0].IsSelected = true;
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.ContainsSingle(x => x.IsSelected, members);
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members[1].IsSelected = true;
        Assert.IsNull(group.Leader.Value);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(2, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));

        members[2].IsSelected = true;
        Assert.IsTrue(group.Leader.Value is true);
        Assert.AreEqual(leader.IsSelected, group.Leader.Value);
        Assert.AreEqual(3, members.Count(x => x.IsSelected));
        Assert.AreEqual(group.SelectedCount, members.Count(x => x.IsSelected));
    }
}
