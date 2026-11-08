using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 在校验前冻结订阅并保护同一对象的整次属性写入, 包括无变化写入.
/// </summary>
internal readonly ref struct ObservablePropertyMutation
{
    private readonly ref bool _active;

    internal ObservablePropertyMutation(
        ref bool active,
        PropertyChangingEventHandler? changing,
        PropertyChangedEventHandler? changed
    )
    {
        if (active)
            throw new InvalidOperationException(
                "Cannot modify this object during an active property mutation."
            );
        _active = ref active;
        _active = true;
        Changing = changing;
        Changed = changed;
    }

    internal PropertyChangingEventHandler? Changing { get; }
    internal PropertyChangedEventHandler? Changed { get; }

    public void Dispose() => _active = false;
}
