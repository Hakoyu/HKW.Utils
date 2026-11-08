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
    private TState _state = default!;

    private Operation BeginOperation()
    {
        if (_changing)
            throw new InvalidOperationException("Reentrant state group writes are not supported.");
        _changing = true;
        return new(this, PropertyChanging, PropertyChanged);
    }

    private readonly struct Operation(
        ObservableStateGroup<TState, TMember> owner,
        PropertyChangingEventHandler? changing,
        PropertyChangedEventHandler? changed
    ) : IDisposable
    {
        public void Changing() => changing?.Invoke(owner, PropertyChangingArgsCache.State);

        public void Commit(TState state)
        {
            var different = EqualityComparer<TState>.Default.Equals(owner._state, state) is false;
            owner._state = state;
            if (different)
                changed?.Invoke(owner, PropertyChangedArgsCache.State);
        }

        public void Dispose() => owner._changing = false;
    }

    /// <inheritdoc/>
    /// <param name="members">成员</param>
    /// <param name="initialize">初始化, 若为 <see langword="false"/> 则必须在 ctor 中手动调用 <see cref="Initialize"/></param>
    protected ObservableStateGroup(IEnumerable<TMember> members, bool initialize)
    {
        ArgumentNullException.ThrowIfNull(members);
        _members = members.ToHashSet();
        foreach (var item in _members)
            ArgumentNullException.ThrowIfNull(item);
        Members = new(_members);
        foreach (var item in _members)
            item.PropertyChanged += Item_PropertyChanged;
        try
        {
            if (initialize)
                Initialize();
        }
        catch
        {
            foreach (var item in _members)
                item.PropertyChanged -= Item_PropertyChanged;
            throw;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected void Initialize()
    {
        using var operation = BeginOperation();
        operation.Changing();
        operation.Commit(InitializeState(Members));
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changing)
            return;
        using var operation = BeginOperation();
        operation.Changing();
        operation.Commit(MemberPropertyChanged((TMember)sender!, e));
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
    /// <remarks>
    /// 新状态
    /// </remarks>
    protected abstract TState StateChanged(TState state, ICollection<TMember> members);

    /// <summary>
    /// 状态
    /// </summary>
    public TState State
    {
        get => _state;
        set
        {
            using var operation = BeginOperation();
            if (EqualityComparer<TState>.Default.Equals(_state, value))
                return;
            operation.Changing();
            operation.Commit(StateChanged(value, Members));
        }
    }

    /// <inheritdoc/>
    public int Count => _members.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(TMember item)
    {
        using var operation = BeginOperation();
        ArgumentNullException.ThrowIfNull(item);
        if (_members.Contains(item))
            return;
        operation.Changing();
        _members.Add(item);
        TState state;
        try
        {
            state = MemberAdded(item);
        }
        catch
        {
            _members.Remove(item);
            throw;
        }
        item.PropertyChanged += Item_PropertyChanged;
        operation.Commit(state);
    }

    /// <inheritdoc/>
    public bool Remove(TMember item)
    {
        using var operation = BeginOperation();
        if (_members.TryGetValue(item, out var stored) is false)
            return false;
        operation.Changing();
        _members.Remove(stored);
        TState state;
        try
        {
            state = MemberRemoved(stored);
        }
        catch
        {
            _members.Add(stored);
            throw;
        }
        stored.PropertyChanged -= Item_PropertyChanged;
        operation.Commit(state);
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var operation = BeginOperation();
        if (_members.Count == 0)
            return;
        operation.Changing();
        var state = MemberClearing(Members);
        foreach (var item in _members)
            item.PropertyChanged -= Item_PropertyChanged;
        _members.Clear();
        operation.Commit(state);
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
