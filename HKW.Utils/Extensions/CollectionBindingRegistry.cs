using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Extensions;

// 弱键表不让静态注册表延长源的生命周期. 活跃源拥有目标, 令牌释放后不再保留两端.
internal static class CollectionBindingRegistry
{
    private static readonly ConditionalWeakTable<object, Group> Groups = new();

    internal static IDisposable Bind<TArgs>(
        object source,
        object target,
        bool remove,
        Func<Action<TArgs>> initialize,
        Action<Channel<TArgs>> subscribe,
        Action<Channel<TArgs>> unsubscribe
    )
        where TArgs : EventArgs
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        if (remove)
        {
            if (Groups.TryGetValue(source, out var existing))
                existing.Entries.Find(e => ReferenceEquals(e.Target, target))?.Dispose();
            return EmptyToken.Instance;
        }
        if (
            source is ICollectionNotificationState { IsNotifying: true }
            || (
                Groups.TryGetValue(source, out var activeGroup)
                && activeGroup
                    .Channels.Values.Cast<ICollectionNotificationState>()
                    .Any(c => c.IsNotifying)
            )
        )
            throw new InvalidOperationException(
                "A new binding cannot be created while the source is notifying."
            );
        if (ReferenceEquals(source, target))
            throw new InvalidOperationException("A collection cannot be bound to itself.");
        var group = Groups.GetValue(source, static _ => new());
        if (group.Entries.Exists(e => ReferenceEquals(e.Target, target)))
            throw new InvalidOperationException("The source and target are already bound.");
        if (Reaches(target, source))
            throw new InvalidOperationException("The binding would create a cycle.");
        if (group.Channels.TryGetValue(typeof(TArgs), out var registered) is false)
        {
            var created = new Channel<TArgs>(group, unsubscribe);
            group.Channels.Add(typeof(TArgs), created);
            registered = created;
        }
        var channel = (Channel<TArgs>)registered;
        var entry = new Entry<TArgs>(group, channel, target, initialize);
        group.Entries.Add(entry);
        channel.Entries.Add(entry);
        channel.Invalidate();
        try
        {
            channel.Subscribe(subscribe);
            entry.Initialize();
            return entry;
        }
        catch
        {
            entry.Dispose();
            throw;
        }
    }

    private static bool Reaches(object start, object destination)
    {
        var pending = new Stack<object>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        pending.Push(start);
        while (pending.TryPop(out var current))
        {
            if (ReferenceEquals(current, destination))
                return true;
            if (
                visited.Add(current) is false
                || Groups.TryGetValue(current, out var group) is false
            )
                continue;
            foreach (var entry in group.Entries)
                if (entry.Target is { } target)
                    pending.Push(target);
        }
        return false;
    }

    internal sealed class Group
    {
        internal readonly List<Entry> Entries = [];
        internal readonly Dictionary<Type, object> Channels = [];
    }

    internal abstract class Entry : IDisposable
    {
        internal object? Target { get; set; }
        public abstract void Dispose();
    }

    internal sealed class Entry<TArgs> : Entry
        where TArgs : EventArgs
    {
        private Group? _group;
        private Channel<TArgs>? _channel;
        private Func<Action<TArgs>>? _initialize;
        private Action<TArgs>? _apply;
        private bool _busy;
        internal bool IsDisposed => _group is null;

        internal Entry(
            Group group,
            Channel<TArgs> channel,
            object target,
            Func<Action<TArgs>> initialize
        )
        {
            _group = group;
            _channel = channel;
            _initialize = initialize;
            Target = target;
        }

        internal void Initialize()
        {
            if (_initialize is null)
                return;
            if (_busy)
                throw new InvalidOperationException(
                    "Reentrant binding initialization is not allowed."
                );
            _busy = true;
            try
            {
                var apply = _initialize();
                if (IsDisposed is false)
                {
                    _apply = apply;
                    _channel!.Invalidate();
                }
                _initialize = null;
            }
            finally
            {
                _busy = false;
            }
        }

        internal Action<TArgs> Capture()
        {
            var apply = _apply;
            return args =>
            {
                if (_busy)
                    throw new InvalidOperationException(
                        "Reentrant binding updates are not allowed."
                    );
                _busy = true;
                try
                {
                    if (apply is null)
                        throw new InvalidOperationException(
                            "The source changed during binding initialization."
                        );
                    apply(args);
                }
                catch
                {
                    Dispose();
                    throw;
                }
                finally
                {
                    _busy = false;
                }
            };
        }

        public override void Dispose()
        {
            if (_group is null)
                return;
            var owner = _channel!;
            _group.Entries.Remove(this);
            owner.Entries.Remove(this);
            owner.Invalidate();
            _group = null;
            _channel = null;
            _initialize = null;
            _apply = null;
            Target = null;
            owner.DetachIfEmpty();
        }
    }

    internal sealed class Channel<TArgs>(Group group, Action<Channel<TArgs>> unsubscribe)
        : ICollectionNotificationSnapshot,
            ICollectionNotificationState
        where TArgs : EventArgs
    {
        internal readonly List<Entry<TArgs>> Entries = [];
        private Group? _group = group;
        private Action<Channel<TArgs>>? _unsubscribe = unsubscribe;
        private Action<TArgs>[]? _snapshot;
        private Action<TArgs>[]? _currentSnapshot;

        internal void Invalidate() => _currentSnapshot = null;

        private bool _subscribed;
        private bool _dispatching;
        internal bool IsCaptured => _snapshot is not null;
        bool ICollectionNotificationState.IsNotifying => IsCaptured;

        internal void Subscribe(Action<Channel<TArgs>> subscribe)
        {
            if (_subscribed)
                return;
            _subscribed = true;
            subscribe(this);
        }

        public bool CaptureNotifications()
        {
            if (_snapshot is not null)
                return false;
            _snapshot = _currentSnapshot ??= Entries.Select(e => e.Capture()).ToArray();
            return true;
        }

        public void ReleaseNotifications()
        {
            _snapshot = null;
            ForgetIfEmpty();
        }

        internal void Handle(object? sender, TArgs args)
        {
            if (_dispatching)
                throw new InvalidOperationException(
                    "Reentrant source notifications are not allowed."
                );
            var ownsSnapshot = CaptureNotifications();
            _dispatching = true;
            List<Exception>? errors = null;
            try
            {
                foreach (var apply in _snapshot!)
                {
                    try
                    {
                        apply(args);
                    }
                    catch (Exception error)
                    {
                        (errors ??= []).Add(error);
                    }
                }
            }
            finally
            {
                _dispatching = false;
                if (ownsSnapshot)
                {
                    try
                    {
                        ReleaseNotifications();
                    }
                    catch (Exception error)
                    {
                        (errors ??= []).Add(error);
                    }
                }
            }
            ThrowErrors(errors);
        }

        internal void DetachIfEmpty()
        {
            if (Entries.Count != 0)
                return;
            var detach = _unsubscribe;
            try
            {
                if (_subscribed)
                {
                    _subscribed = false;
                    detach?.Invoke(this);
                }
            }
            finally
            {
                ForgetIfEmpty();
            }
        }

        private void ForgetIfEmpty()
        {
            if (Entries.Count != 0 || IsCaptured)
                return;
            _group?.Channels.Remove(typeof(TArgs));
            _group = null;
            _unsubscribe = null;
        }
    }

    private static void ThrowErrors(List<Exception>? errors)
    {
        if (errors is null)
            return;
        if (errors.Count == 1)
            ExceptionDispatchInfo.Capture(errors[0]).Throw();
        throw new AggregateException("Multiple collection binding updates failed.", errors);
    }

    private sealed class EmptyToken : IDisposable
    {
        internal static readonly EmptyToken Instance = new();

        public void Dispose() { }
    }
}
