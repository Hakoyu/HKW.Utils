﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class SelectionGroupTests
{
    [TestMethod]
    public void InitializeAllMemberTrue()
    {
        var leader = new LeaderModel();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new SelectableModel() { IsSelected = true })
            .ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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
        var leader = new LeaderModel();
        var members = Enumerable.Range(0, 3).Select(_ => new SelectableModel()).ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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
        var leader = new LeaderModel();
        var members = Enumerable.Range(0, 3).Select(_ => new SelectableModel()).ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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
        var leader = new LeaderModel();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new SelectableModel() { IsSelected = true })
            .ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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
        var leader = new LeaderModel();
        var members = Enumerable.Range(0, 3).Select(_ => new SelectableModel()).ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Add(new());
        group.MemberWrapperByMember.Add(
            members.Last(),
            group.MemberWrapperByMember.Values.First().Clone(members.Last())
        );
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Add(new() { IsSelected = true });
        group.MemberWrapperByMember.Add(
            members.Last(),
            group.MemberWrapperByMember.Values.First().Clone(members.Last())
        );
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
        var leader = new LeaderModel();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new SelectableModel() { IsSelected = true })
            .ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Add(new());
        group.MemberWrapperByMember.Add(
            members.Last(),
            group.MemberWrapperByMember.Values.First().Clone(members.Last())
        );
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Add(new() { IsSelected = true });
        group.MemberWrapperByMember.Add(
            members.Last(),
            group.MemberWrapperByMember.Values.First().Clone(members.Last())
        );
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
        var leader = new LeaderModel();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new SelectableModel() { IsSelected = true })
            .ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            members
        );
        members.Remove(members.Last());
        group.MemberWrapperByMember.Remove(group.MemberWrapperByMember.Last());
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Remove(members.Last());
        group.MemberWrapperByMember.Remove(group.MemberWrapperByMember.Last());
        Assert.IsTrue(group.Leader.Value == leader.IsSelected);
        Assert.IsTrue(
            members.Count(x => x.IsSelected)
                == group.MemberWrapperByMember.Values.Count(x => x.Value)
        );
        Assert.IsTrue(members.Count(x => x.IsSelected) == group.SelectedCount);

        members.Remove(members.Last());
        group.MemberWrapperByMember.Remove(group.MemberWrapperByMember.Last());
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
        var leader = new LeaderModel();
        var members = Enumerable
            .Range(0, 3)
            .Select(_ => new SelectableModel() { IsSelected = true })
            .ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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
        var leader = new LeaderModel();
        var members = Enumerable.Range(0, 3).Select(_ => new SelectableModel()).ToList();
        var group = new SelectionGroup<LeaderModel, SelectableModel>(
            new(
                leader,
                nameof(LeaderModel.IsSelected),
                x => x.IsSelected,
                (x, v) => x.IsSelected = v
            ),
            new(
                new(),
                nameof(SelectableModel.IsSelected),
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

[HKWReactiveUI.ReferenceType(typeof(ReactiveObject))]
internal partial class LeaderModel : ReactiveObjectX
{
    [ReactiveProperty]
    public bool? IsSelected { get; set; }
}

internal partial class SelectableModel : ReactiveObjectX
{
    [ReactiveProperty]
    public bool IsSelected { get; set; }
}