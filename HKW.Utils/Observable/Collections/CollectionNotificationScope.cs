using System.Collections.Specialized;
using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 转发视图在源操作开始时一起冻结订阅, 不能等收到首个事件才捕获.
/// </summary>
internal interface ICollectionNotificationSnapshot
{
    bool CaptureNotifications();
    void ReleaseNotifications();
}

internal sealed class CollectionNotificationScope<TChanging, TChanged>
    where TChanging : Delegate
    where TChanged : Delegate
{
    public bool IsActive { get; private set; }
    public TChanging? Changing { get; private set; }
    public TChanged? Changed { get; private set; }
    public NotifyCollectionChangedEventHandler? Collection { get; private set; }
    public PropertyChangedEventHandler? Property { get; private set; }
    private List<ICollectionNotificationSnapshot>? _participants;

    public Lease Enter(
        TChanging? changing,
        TChanged? changed,
        NotifyCollectionChangedEventHandler? collection,
        PropertyChangedEventHandler? property
    )
    {
        if (IsActive)
            throw new InvalidOperationException(
                "Reentrant writes to the same collection are not allowed during notification."
            );
        IsActive = true;
        Changing = changing;
        Changed = changed;
        Collection = collection;
        Property = property;
        try
        {
            Capture(changing);
            Capture(changed);
            Capture(collection);
            Capture(property);
            return new(this);
        }
        catch
        {
            Exit();
            throw;
        }
    }

    private void Capture(Delegate? handlers)
    {
        if (handlers is null)
            return;
        foreach (var handler in Delegate.EnumerateInvocationList(handlers))
            Include(handler.Target as ICollectionNotificationSnapshot);
    }

    public void Include(ICollectionNotificationSnapshot? participant)
    {
        if (participant?.CaptureNotifications() is true)
            (_participants ??= []).Add(participant);
    }

    public void Exit()
    {
        if (_participants is not null)
        {
            foreach (var participant in _participants)
                participant.ReleaseNotifications();
            _participants.Clear();
        }
        Changing = null;
        Changed = null;
        Collection = null;
        Property = null;
        IsActive = false;
    }

    public readonly struct Lease(CollectionNotificationScope<TChanging, TChanged> owner)
        : IDisposable
    {
        public void Dispose() => owner.Exit();
    }
}
