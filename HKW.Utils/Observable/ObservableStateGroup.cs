using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测的状态组
/// </summary>
/// <typeparam name="TState">状态类型</typeparam>
/// <typeparam name="TMember">成员</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public abstract class ObservableStateGroup<TState, TMember>
    : INotifyPropertyChanging,
        INotifyPropertyChanged,
        ICollection<TMember>
    where TMember : INotifyPropertyChanged
{
    private readonly HashSet<TMember> _members;

    /// <summary>
    /// 成员
    /// </summary>
    protected ReadOnlySet<TMember> Members { get; }

    /// <summary>
    /// 正在改变
    /// </summary>
    private bool _changing = false;

    /// <inheritdoc/>
    /// <param name="members">成员</param>
    /// <param name="initialize">初始化, 若为 <see langword="false"/> 则必须在 ctor 中手动调用 <see cref="Initialize"/></param>
    protected ObservableStateGroup(IEnumerable<TMember> members, bool initialize)
    {
        ArgumentNullException.ThrowIfNull(members);
        State = default!;
        _members = members.ToHashSet();
        Members = new(_members);
        foreach (var item in _members)
            item.PropertyChanged += Item_PropertyChanged;
        if (initialize)
        {
            Initialize();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected void Initialize()
    {
        _changing = true;
        try
        {
            State = InitializeState(Members);
        }
        finally
        {
            _changing = false;
        }
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        State = MemberPropertyChanged((TMember)sender!, e);
    }

    /// <summary>
    /// 初始化状态, 子类在此初始化不要使用 Ctor
    /// </summary>
    /// <param name="members">成员</param>
    /// <returns>状态</returns>
    protected abstract TState InitializeState(ICollection<TMember> members);

    /// <summary>
    /// 成员属性改变
    /// </summary>
    /// <param name="member">成员</param>
    /// <param name="e">属性改变事件参数</param>
    /// <returns>状态</returns>
    protected abstract TState MemberPropertyChanged(TMember member, PropertyChangedEventArgs e);

    /// <summary>
    /// 成员被添加
    /// </summary>
    /// <param name="member">成员</param>
    /// <returns>状态</returns>
    protected abstract TState MemberAdded(TMember member);

    /// <summary>
    /// 成员被删除
    /// </summary>
    /// <param name="member">成员</param>
    /// <returns>状态</returns>
    protected abstract TState MemberRemoved(TMember member);

    /// <summary>
    /// 成员被清理
    /// </summary>
    /// <param name="members">成员</param>
    /// <returns>状态</returns>
    protected abstract TState MemberClearing(ICollection<TMember> members);

    /// <summary>
    /// 状态改变
    /// </summary>
    /// <param name="state">状态</param>
    /// <param name="members">成员</param>
    /// <remarks>新状态</remarks>
    protected abstract TState StateChanged(TState state, ICollection<TMember> members);

    /// <summary>
    /// 状态
    /// </summary>
    public TState State
    {
        get => field;
        set
        {
            if (EqualityComparer<TState>.Default.Equals(field, value))
                return;
            PropertyChanging?.Invoke(this, PropertyChangingEventArgs.Cache_State);
            field = value;
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_State);

            if (_changing)
                return;
            _changing = true;
            try
            {
                field = StateChanged(value, Members);
            }
            finally
            {
                _changing = false;
            }
        }
    }

    /// <inheritdoc/>
    public int Count => _members.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(TMember item)
    {
        if (_members.Add(item))
        {
            item.PropertyChanged += Item_PropertyChanged;
            _changing = true;
            try
            {
                State = MemberAdded(item);
            }
            finally
            {
                _changing = false;
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(TMember item)
    {
        var result = _members.Remove(item);
        if (result)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            _changing = true;
            try
            {
                State = MemberRemoved(item);
            }
            finally
            {
                _changing = false;
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        foreach (var item in _members)
            item.PropertyChanged -= Item_PropertyChanged;
        _changing = true;
        try
        {
            State = MemberClearing(Members);
        }
        finally
        {
            _changing = false;
        }
        _members.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TMember item)
    {
        return _members.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TMember[] array, int arrayIndex)
    {
        _members.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TMember> GetEnumerator()
    {
        return _members.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_members).GetEnumerator();
    }

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
