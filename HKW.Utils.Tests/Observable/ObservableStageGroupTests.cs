using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using ReactiveUI;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
[ReferenceType(typeof(ReactiveObject))]
public class ObservableStageGroupTests
{
    [TestMethod]
    public void InitializeAllMemberSelectedTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.Stage);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void InitializeAllMemberSelectedFalse()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.Stage);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStageChengeTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.Stage);
        group.Stage = true;
        Assert.IsTrue(group.Stage);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStageChengeFalse()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.Stage);
        group.Stage = false;
        Assert.IsFalse(group.Stage);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStageChengeNull()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.Stage);
        group.Stage = null;
        Assert.IsTrue(group.Stage);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void MemberSelectedChangeTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.Stage);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        foreach (var (e, i) in group.WithIndex())
        {
            e.IsSelected = true;
            if (i < group.Count - 1)
                Assert.IsNull(group.Stage);
            Assert.AreEqual(i + 1, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        }
        Assert.IsTrue(group.Stage);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void MemberSelectedChangeFalue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.Stage);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        foreach (var (e, i) in group.WithIndex())
        {
            e.IsSelected = false;
            if (i < group.Count - 1)
                Assert.IsNull(group.Stage);
            Assert.AreEqual(
                group.Count - i - 1,
                [group.SelectedCount, group.Count(x => x.IsSelected)]
            );
        }
        Assert.IsFalse(group.Stage);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }
}

public partial class ObservableSelectableGroup : ObservableStageGroup<bool?, ObservableSelectable>
{
    public ObservableSelectableGroup(IEnumerable<ObservableSelectable> members)
        : base(members, false)
    {
        _selectedByMember = new(Count);
        Initialize();
    }

    public int SelectedCount { get; private set; }

    private Dictionary<ObservableSelectable, bool> _selectedByMember;

    private bool? GetStage()
    {
        if (Count == 0)
            return false;
        else if (SelectedCount == Count)
            return true;
        else if (SelectedCount == 0)
            return false;
        else
            return null;
    }

    protected override bool? InitializeStage(ICollection<ObservableSelectable> members)
    {
        foreach (var item in members)
        {
            if (item.IsSelected)
                SelectedCount++;
            _selectedByMember.Add(item, item.IsSelected);
        }
        return GetStage();
    }

    protected override bool? MemberPropertyChanged(
        ObservableSelectable member,
        PropertyChangedEventArgs e
    )
    {
        if (e.PropertyName != nameof(member.IsSelected))
            return Stage;
        if (member.IsSelected && _selectedByMember[member] is false)
        {
            SelectedCount++;
            _selectedByMember[member] = true;
        }
        else if (member.IsSelected is false && _selectedByMember[member])
        {
            SelectedCount--;
            _selectedByMember[member] = false;
        }
        return GetStage();
    }

    protected override bool? StageChanged(bool? stage, ICollection<ObservableSelectable> members)
    {
        if (stage is true)
        {
            foreach (var item in members)
            {
                item.IsSelected = true;
                _selectedByMember[item] = true;
            }
            SelectedCount = Count;
        }
        else if (stage is false)
        {
            foreach (var item in members)
            {
                item.IsSelected = false;
                _selectedByMember[item] = false;
            }
            SelectedCount = 0;
        }
        return GetStage();
    }

    protected override bool? MemberAdded(ObservableSelectable member)
    {
        if (member.IsSelected)
            SelectedCount++;
        _selectedByMember[member] = member.IsSelected;
        return GetStage();
    }

    protected override bool? MemberRemoved(ObservableSelectable member)
    {
        if (member.IsSelected)
            SelectedCount--;
        _selectedByMember.Remove(member);
        return GetStage();
    }

    protected override bool? MemberClearing(ICollection<ObservableSelectable> members)
    {
        _selectedByMember.Clear();
        SelectedCount = 0;
        return false;
    }
}

public partial class ObservableSelectable : ReactiveObject
{
    [ReactiveProperty]
    public bool IsSelected { get; set; }
}
