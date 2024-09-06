using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using HKW.HKWReactiveUI;
using ReactiveUI;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 选择组
/// </summary>
public partial class SelectionGroup<TLeader, TMember> : ReactiveObjectX, IDisposable
    where TLeader : INotifyPropertyChanged
    where TMember : INotifyPropertyChanged
{
    /// <inheritdoc/>
    /// <param name="leaderWrapper">队长包装器</param>
    /// <param name="memberWrapper">成员包装器</param>
    /// <param name="members">成员</param>
    public SelectionGroup(
        ObservablePropertyWrapper<TLeader, bool?> leaderWrapper,
        ObservablePropertyWrapper<TMember, bool> memberWrapper,
        IEnumerable<TMember> members
    )
    {
        Leader = leaderWrapper;
        foreach (var member in members)
        {
            member.PropertyChanged -= Member_PropertyChanged;
            member.PropertyChanged += Member_PropertyChanged;
            MemberWrapperByMember.Add(member, memberWrapper.Clone(member));
        }
        RefreshLeader();

        MemberWrapperByMember.DictionaryChanged -= MemberWrapperByMember_DictionaryChanged;
        MemberWrapperByMember.DictionaryChanged += MemberWrapperByMember_DictionaryChanged;
        Leader.PropertyChanged -= Leader_PropertyChanged;
        Leader.PropertyChanged += Leader_PropertyChanged;
    }

    /// <summary>
    /// 组长
    /// </summary>
    public ObservablePropertyWrapper<TLeader, bool?> Leader { get; }

    /// <summary>
    /// 成员
    /// </summary>
    public ObservableDictionary<
        TMember,
        ObservablePropertyWrapper<TMember, bool>
    > MemberWrapperByMember { get; } = [];

    /// <summary>
    /// 选中的数量
    /// </summary>
    public int SelectedCount { get; private set; }

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
                Leader.Value = null;
            else if (
                (MemberWrapperByMember.Count == 1)
                || MemberWrapperByMember.Count == SelectedCount - 1
            )
                Leader.Value = true;
            SelectedCount++;
        }
        else if (wrapper.Value is false)
        {
            if ((Leader.Value is true) || (SelectedCount == 1))
                Leader.Value = false;
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

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~SelectionGroup()
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

        if (disposing) { }

        Leader.Dispose();
        Leader.PropertyChanged -= Leader_PropertyChanged;
        foreach (var pair in MemberWrapperByMember)
        {
            pair.Value.Dispose();
            pair.Value.PropertyChanged -= Member_PropertyChanged;
        }
        MemberWrapperByMember.DictionaryChanged -= MemberWrapperByMember_DictionaryChanged;
        MemberWrapperByMember.Clear();

        _disposed = true;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Close()
    {
        Dispose();
    }
    #endregion
}

/// <summary>
/// 可观察属性包装器
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public partial class ObservablePropertyWrapper<TSource, TValue>
    : ReactiveObjectX,
        ICloneable<ObservablePropertyWrapper<TSource, TValue>>,
        IDisposable
    where TSource : INotifyPropertyChanged
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="propertyName">属性名</param>
    /// <param name="getAction">获取行动</param>
    /// <param name="setAction">设置行动</param>
    public ObservablePropertyWrapper(
        TSource source,
        string propertyName,
        Func<TSource, TValue> getAction,
        Action<TSource, TValue> setAction
    )
    {
        Source = source;
        PropertyName = propertyName;
        GetAction = getAction;
        SetAction = setAction;
        Source.PropertyChanged -= Source_PropertyChanged;
        Source.PropertyChanged += Source_PropertyChanged;
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PropertyName)
            this.RaisePropertyChanged(PropertyName);
    }

    /// <summary>
    /// 源
    /// </summary>
    public TSource Source { get; }

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue Value
    {
        get => GetAction.Invoke(Source);
        set => SetAction.Invoke(Source, value);
    }

    /// <summary>
    /// 获取行动
    /// </summary>
    public Func<TSource, TValue> GetAction { get; set; }

    /// <summary>
    /// 设置行动
    /// </summary>
    public Action<TSource, TValue> SetAction { get; set; }

    #region ICloneable
    /// <inheritdoc/>
    public ObservablePropertyWrapper<TSource, TValue> Clone()
    {
        return Clone(Source);
    }

    /// <summary>
    /// 克隆一个新对象, 但重新设置源
    /// </summary>
    /// <param name="source">新源</param>
    /// <returns>克隆的对象</returns>
    public ObservablePropertyWrapper<TSource, TValue> Clone(TSource source)
    {
        return new(source, PropertyName, GetAction, SetAction);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
    #endregion

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ObservablePropertyWrapper()
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

        if (disposing) { }

        Source.PropertyChanged -= Source_PropertyChanged;

        _disposed = true;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Close()
    {
        Dispose();
    }
    #endregion
}
