using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableStateGroupTests
{
    [TestMethod]
    public void InitializeAllMemberSelectedTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.State);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void InitializeAllMemberSelectedFalse()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.State);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStateChengeTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.State);
        group.State = true;
        Assert.IsTrue(group.State);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStateChengeFalse()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.State);
        group.State = false;
        Assert.IsFalse(group.State);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void GroupStateChengeNull()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.State);
        group.State = null;
        Assert.IsTrue(group.State);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void MemberSelectedChangeTrue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = false })
        );
        Assert.IsFalse(group.State);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        foreach (var (e, i) in group.WithIndex())
        {
            e.IsSelected = true;
            if (i < group.Count - 1)
                Assert.IsNull(group.State);
            Assert.AreEqual(i + 1, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        }
        Assert.IsTrue(group.State);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }

    [TestMethod]
    public void MemberSelectedChangeFalue()
    {
        var group = new ObservableSelectableGroup(
            Enumerable.Range(0, 10).Select(_ => new ObservableSelectable() { IsSelected = true })
        );
        Assert.IsTrue(group.State);
        Assert.AreEqual(10, [group.SelectedCount, group.Count(x => x.IsSelected)]);
        foreach (var (e, i) in group.WithIndex())
        {
            e.IsSelected = false;
            if (i < group.Count - 1)
                Assert.IsNull(group.State);
            Assert.AreEqual(
                group.Count - i - 1,
                [group.SelectedCount, group.Count(x => x.IsSelected)]
            );
        }
        Assert.IsFalse(group.State);
        Assert.AreEqual(0, [group.SelectedCount, group.Count(x => x.IsSelected)]);
    }
}

public partial class ObservableSelectableGroup : ObservableStateGroup<bool?, ObservableSelectable>
{
    public ObservableSelectableGroup(IEnumerable<ObservableSelectable> members)
        : base(members, false)
    {
        _selectedByMember = new(Count);
        Initialize();
    }

    public int SelectedCount { get; private set; }

    private readonly Dictionary<ObservableSelectable, bool> _selectedByMember;

    private bool? GetState()
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

    protected override bool? InitializeState(ICollection<ObservableSelectable> members)
    {
        foreach (var item in members)
        {
            if (item.IsSelected)
                SelectedCount++;
            _selectedByMember.Add(item, item.IsSelected);
        }
        return GetState();
    }

    protected override bool? MemberPropertyChanged(
        ObservableSelectable member,
        PropertyChangedEventArgs e
    )
    {
        if (e.PropertyName != nameof(member.IsSelected))
            return State;
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
        return GetState();
    }

    protected override bool? StateChanged(bool? stage, ICollection<ObservableSelectable> members)
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
        return GetState();
    }

    protected override bool? MemberAdded(ObservableSelectable member)
    {
        if (member.IsSelected)
            SelectedCount++;
        _selectedByMember[member] = member.IsSelected;
        return GetState();
    }

    protected override bool? MemberRemoved(ObservableSelectable member)
    {
        if (member.IsSelected)
            SelectedCount--;
        _selectedByMember.Remove(member);
        return GetState();
    }

    protected override bool? MemberClearing(ICollection<ObservableSelectable> members)
    {
        _selectedByMember.Clear();
        SelectedCount = 0;
        return false;
    }
}

public partial class ObservableSelectable : INotifyPropertyChanged
{
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;
            _isSelected = value;
            PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
