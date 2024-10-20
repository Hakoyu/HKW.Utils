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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Add(new() { IsSelected = true });
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Add(new() { IsSelected = true });
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Remove(members.Last());
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Remove(members.Last());
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members[1].IsSelected = false;
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members[2].IsSelected = false;
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
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
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members[1].IsSelected = true;
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members[2].IsSelected = true;
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);
    }
}
