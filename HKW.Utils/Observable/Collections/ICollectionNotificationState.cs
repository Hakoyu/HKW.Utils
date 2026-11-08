namespace HKW.HKWUtils.Observable;

/// <summary>
/// 集合操作通知状态的内部只读桥, 不允许修改订阅快照.
/// </summary>
internal interface ICollectionNotificationState
{
    bool IsNotifying { get; }
}
