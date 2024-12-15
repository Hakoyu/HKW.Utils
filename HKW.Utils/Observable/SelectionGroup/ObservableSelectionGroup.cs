using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using HKW.HKWReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测的选择组
/// </summary>
/// <typeparam name="TMember">成员类型</typeparam>
[DebuggerDisplay("Leader = {Leader.Value}, Count = {Count}, SelectedCount = {SelectedCount}")]
public partial class ObservableSelectionGroup<TMember>
    : ObservableSelectionGroup<
        ObservableSelectionGroupLeader,
        ObservableSelectionGroupMember<TMember>,
        ObservableList<ObservableSelectionGroupMember<TMember>>
    >
{
    /// <summary>
    ///
    /// </summary>
    public ObservableSelectionGroup()
        : base(
            new ObservableSelectionGroupLeader().Wrapper,
            new ObservableSelectionGroupMember<TMember>().Wrapper,
            new ObservableList<ObservableSelectionGroupMember<TMember>>()
        ) { }

    /// <summary>
    ///
    /// </summary>
    public ObservableSelectionGroup(IEnumerable<TMember> collection)
        : base(
            new ObservableSelectionGroupLeader().Wrapper,
            new ObservableSelectionGroupMember<TMember>().Wrapper,
            new ObservableList<ObservableSelectionGroupMember<TMember>>()
        )
    {
        foreach (var item in collection)
            Members.Add(new(item));
    }
}

/// <summary>
/// 可观测的选择组
/// </summary>
/// <typeparam name="TLeader">队长类型</typeparam>
/// <typeparam name="TMember">成员类型</typeparam>
/// <typeparam name="TMemberCollection">成员集合类型</typeparam>
[DebuggerDisplay("Leader = {Leader.Value}, Count = {Count}, SelectedCount = {SelectedCount}")]
public partial class ObservableSelectionGroup<TLeader, TMember, TMemberCollection>
    : ReactiveObjectX,
        ICollection<TMember>,
        IDisposable
    where TLeader : INotifyPropertyChanged
    where TMember : INotifyPropertyChanged
    where TMemberCollection : IObservableCollection<TMember>
{
    /// <inheritdoc/>
    /// <param name="leaderWrapper">队长包装器</param>
    /// <param name="memberWrapper">成员包装器</param>
    /// <param name="memberCollection">成员</param>
    public ObservableSelectionGroup(
        ObservablePropertyWrapper<TLeader, bool?> leaderWrapper,
        ObservablePropertyWrapper<TMember, bool> memberWrapper,
        TMemberCollection memberCollection
    )
    {
        Leader = leaderWrapper;
        Members = memberCollection;
        _memberWrapper = memberWrapper.Clone(default!);
        foreach (var member in Members)
        {
            member.PropertyChanged -= Member_PropertyChanged;
            member.PropertyChanged += Member_PropertyChanged;
        }
        RefreshLeader();

        Leader.PropertyChanged -= Leader_PropertyChanged;
        Leader.PropertyChanged += Leader_PropertyChanged;

        Members.CollectionChanged -= ObservableMembers_CollectionChanged;
        Members.CollectionChanged += ObservableMembers_CollectionChanged;
    }

    /// <summary>
    /// 组长
    /// </summary>
    public ObservablePropertyWrapper<TLeader, bool?> Leader { get; }

    /// <summary>
    /// 可观测成员
    /// </summary>
    public TMemberCollection Members { get; }

    /// <summary>
    /// 选中的数量
    /// </summary>
    public int SelectedCount { get; private set; }

    private ObservablePropertyWrapper<TMember, bool> _memberWrapper;
    private bool _changing;

    /// <summary>
    /// 刷新组长
    /// </summary>
    public void RefreshLeader()
    {
        _changing = true;
        SelectedCount = Members.Count(x => _memberWrapper.GetAction(x));
        Leader.Value = SelectedCount == 0 ? false : (SelectedCount == Members.Count ? true : null);
        _changing = false;
    }

    private void Member_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        _changing = true;
        if (sender is not TMember member)
            return;
        var selected = _memberWrapper.GetAction(member);

        if (selected)
            SelectedCount++;
        else if (selected is false)
            SelectedCount--;

        if (SelectedCount == Members.Count && Members.Count != 0)
            Leader.Value = true;
        else if (SelectedCount != 0)
            Leader.Value = null;
        else
            Leader.Value = false;
        _changing = false;
    }

    private void Leader_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        _changing = true;
        if (Leader.Value is true)
        {
            foreach (var item in Members)
                _memberWrapper.SetAction(item, true);
            SelectedCount = Members.Count;
        }
        else if (Leader.Value is false)
        {
            foreach (var item in Members)
                _memberWrapper.SetAction(item, false);
            SelectedCount = 0;
        }
        _changing = false;
    }

    private void ObservableMembers_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        _changing = true;

        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems!.Cast<TMember>())
            {
                item.PropertyChanged -= Member_PropertyChanged;

                var selected = _memberWrapper.GetAction(item);
                if (selected)
                    SelectedCount--;
            }
        }
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems!.Cast<TMember>())
            {
                item.PropertyChanged -= Member_PropertyChanged;
                item.PropertyChanged += Member_PropertyChanged;

                var selected = _memberWrapper.GetAction(item);
                if (selected is true)
                    SelectedCount++;
            }
        }
        if (SelectedCount == Members.Count && Members.Count != 0)
            Leader.Value = true;
        else if (SelectedCount != 0)
            Leader.Value = null;
        else
            Leader.Value = false;
        _changing = false;
    }

    #region ICollection
    /// <inheritdoc/>
    public int Count => Members.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => Members.IsReadOnly;

    /// <inheritdoc/>
    public void Add(TMember item)
    {
        Members.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Members.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TMember item)
    {
        return Members.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TMember[] array, int arrayIndex)
    {
        Members.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(TMember item)
    {
        return Members.Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<TMember> GetEnumerator()
    {
        return Members.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Members).GetEnumerator();
    }
    #endregion

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        base.Dispose(disposing);
        if (disposing)
        {
            Leader.Dispose();
            Leader.PropertyChanged -= Leader_PropertyChanged;
            foreach (var member in Members)
            {
                member.PropertyChanged -= Member_PropertyChanged;
            }
            Members.Clear();
        }
    }
}
