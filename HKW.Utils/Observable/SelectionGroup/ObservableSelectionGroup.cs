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
[DebuggerDisplay("Leader = {Leader.IsSelected}, Count = {Count}")]
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
[DebuggerDisplay("Leader = {Leader.IsSelected}, Count = {Count}")]
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
            MemberWrapperByMember.Add(member, _memberWrapper.Clone(member));
        }
        RefreshLeader();

        MemberWrapperByMember.DictionaryChanged -= MemberWrapperByMember_DictionaryChanged;
        MemberWrapperByMember.DictionaryChanged += MemberWrapperByMember_DictionaryChanged;
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
    /// 成员 (Member, MemberWrapper)
    /// </summary>
    public ObservableDictionary<
        TMember,
        ObservablePropertyWrapper<TMember, bool>
    > MemberWrapperByMember { get; } = [];

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
        SelectedCount = MemberWrapperByMember.Count(x => x.Value.Value);
        Leader.Value =
            SelectedCount == 0
                ? false
                : (SelectedCount == MemberWrapperByMember.Count ? true : null);
        _changing = false;
    }

    private void MemberWrapperByMember_DictionaryChanged(
        IObservableDictionary<TMember, ObservablePropertyWrapper<TMember, bool>> sender,
        NotifyDictionaryChangeEventArgs<TMember, ObservablePropertyWrapper<TMember, bool>> e
    )
    {
        _changing = true;
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var _, out var newMember) is false)
                return;
            newMember.PropertyChanged -= Member_PropertyChanged;
            newMember.PropertyChanged += Member_PropertyChanged;

            if (newMember.Value is true)
            {
                if (Leader.Value is false)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = true;
                    else
                        Leader.Value = null;
                }

                SelectedCount++;
            }
            else if (newMember.Value is false)
            {
                if (Leader.Value is true)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = false;
                    else
                        Leader.Value = null;
                }
            }
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var _, out var oldMember) is false)
                return;
            oldMember.PropertyChanged -= Member_PropertyChanged;

            if (oldMember.Value is true)
            {
                if (SelectedCount == 1)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = false;
                    else
                        Leader.Value = null;
                }
                SelectedCount--;
            }
            else if (oldMember.Value is false)
            {
                if (Leader.Value is true)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = false;
                    else
                        Leader.Value = null;
                }
                else if (SelectedCount == MemberWrapperByMember.Count - 1)
                {
                    Leader.Value = true;
                }
            }
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetNewPair(out var _, out var newMember) is false)
                return;
            if (e.TryGetOldPair(out var _, out var oldMember) is false)
                return;

            newMember.PropertyChanged -= Member_PropertyChanged;
            newMember.PropertyChanged += Member_PropertyChanged;

            oldMember.PropertyChanged -= Member_PropertyChanged;

            if (newMember.Value is true)
            {
                if (Leader.Value is false)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = true;
                    else
                        Leader.Value = null;
                }

                if (oldMember.Value is false)
                    SelectedCount++;
            }
            else if (newMember.Value is false)
            {
                if (Leader.Value is true)
                {
                    if (MemberWrapperByMember.Count == 1)
                        Leader.Value = false;
                    else
                        Leader.Value = null;
                }

                if (oldMember.Value is true)
                    SelectedCount--;
            }
        }
        _changing = false;
    }

    private void Member_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        if (sender is not TMember member)
            return;
        if (MemberWrapperByMember.TryGetValue(member, out var wrapper) is false)
            return;
        _changing = true;
        if (wrapper.Value)
        {
            if (Leader.Value is false)
            {
                if (
                    (MemberWrapperByMember.Count == 1)
                    || (SelectedCount == MemberWrapperByMember.Count - 1)
                )
                    Leader.Value = true;
                else
                    Leader.Value = null;
            }
            SelectedCount++;
        }
        else if (wrapper.Value is false)
        {
            if (Leader.Value is true)
            {
                if (SelectedCount == 1)
                    Leader.Value = false;
                else
                    Leader.Value = null;
            }
            SelectedCount--;
        }
        _changing = false;
    }

    private void Leader_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        _changing = true;
        if (Leader.Value is true)
        {
            foreach (var pair in MemberWrapperByMember)
                pair.Value.Value = true;

            SelectedCount = MemberWrapperByMember.Count;
        }
        else if (Leader.Value is false)
        {
            foreach (var pair in MemberWrapperByMember)
                pair.Value.Value = false;
            SelectedCount = 0;
        }
        _changing = false;
    }

    private void ObservableMembers_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems!.Cast<TMember>())
            {
                MemberWrapperByMember.Add(item, _memberWrapper.Clone(item));
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems!.Cast<TMember>())
            {
                MemberWrapperByMember.Remove(item);
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Replace)
        {
            for (var i = 0; i < e.OldItems!.Count; i++)
            {
                MemberWrapperByMember[(TMember)e.OldItems[i]!] = _memberWrapper.Clone(
                    (TMember)e.NewItems![i]!
                );
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            MemberWrapperByMember.Clear();
        }
    }

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservableSelectionGroup()
    {
        //必须为false
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        //必须为true
        Dispose(true);
        //通知垃圾回收器不再调用终结器
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Leader.Dispose();
            Leader.PropertyChanged -= Leader_PropertyChanged;
            foreach (var pair in MemberWrapperByMember)
            {
                pair.Value.Dispose();
                pair.Value.PropertyChanged -= Member_PropertyChanged;
            }
            MemberWrapperByMember.DictionaryChanged -= MemberWrapperByMember_DictionaryChanged;
            MemberWrapperByMember.Clear();
        }

        _disposed = true;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Close()
    {
        Dispose();
    }
    #endregion

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
}
