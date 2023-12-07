using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 属性改变监听器
/// </summary>
public class PropertyChangeListener
{
    /// <summary>
    /// 通知改变前源
    /// </summary>
    public INotifyPropertyChanging NotifyChangingSource { get; private set; } = null!;

    /// <summary>
    /// 通知改变后源
    /// </summary>
    public INotifyPropertyChanged NotifyChangedSource { get; private set; }

    /// <summary>
    /// 通知的属性名称
    /// </summary>
    public HashSet<string> PropertyNames { get; set; } = new();

    /// <inheritdoc/>
    /// <param name="notifySource">通知源</param>
    public PropertyChangeListener(INotifyPropertyChanged notifySource)
    {
        NotifyChangedSource = notifySource;
        NotifyChangedSource.PropertyChanged += NotifySource_PropertyChanged;
        if (notifySource is INotifyPropertyChanging notifyChanging)
            notifyChanging.PropertyChanging += NotifySource_PropertyChanging;
    }

    /// <summary>
    /// 关闭 (会从通知源删除事件)
    /// </summary>
    public void Close()
    {
        if (NotifyChangingSource is not null)
        {
            NotifyChangingSource.PropertyChanging -= NotifySource_PropertyChanging;
            NotifyChangingSource = null!;
        }
        NotifyChangedSource.PropertyChanged -= NotifySource_PropertyChanged;
        NotifyChangedSource = null!;
    }

    private void NotifySource_PropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (PropertyNames.Contains(e.PropertyName!))
            PropertyChanging?.Invoke(this, e);
    }

    private void NotifySource_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (PropertyNames.Contains(e.PropertyName!))
            PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// 监听的属性更改前
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// 监听的属性更改后
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}
