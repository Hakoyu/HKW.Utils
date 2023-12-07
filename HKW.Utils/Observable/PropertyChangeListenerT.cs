using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 属性改变监听器
/// </summary>
public class PropertyChangeListener<TSource>
{
    /// <summary>
    /// 通知改变前源
    /// </summary>
    public INotifyPropertyChangingX<TSource> NotifyChangingSource { get; private set; } = null!;

    /// <summary>
    /// 通知改变后源
    /// </summary>
    public INotifyPropertyChangedX<TSource> NotifyChangedSource { get; private set; }

    /// <summary>
    /// 通知的属性名称
    /// </summary>
    public HashSet<string> ListeningPropertyNames { get; set; } = new();

    /// <inheritdoc/>
    /// <param name="notifySource">通知源</param>
    public PropertyChangeListener(INotifyPropertyChangedX<TSource> notifySource)
    {
        NotifyChangedSource = notifySource;
        NotifyChangedSource.PropertyChangedX += NotifySource_PropertyChanged;
        if (notifySource is INotifyPropertyChangingX<TSource> notifyChanging)
            notifyChanging.PropertyChangingX += NotifySource_PropertyChanging;
    }

    /// <summary>
    /// 关闭 (会从通知源删除事件)
    /// </summary>
    public void Close()
    {
        if (NotifyChangingSource is not null)
        {
            NotifyChangingSource.PropertyChangingX -= NotifySource_PropertyChanging;
            NotifyChangingSource = null!;
        }
        NotifyChangedSource.PropertyChangedX -= NotifySource_PropertyChanged;
        NotifyChangedSource = null!;
    }

    private void NotifySource_PropertyChanging(TSource? sender, PropertyChangingXEventArgs e)
    {
        if (ListeningPropertyNames.Contains(e.PropertyName!))
            ListenedPropertyChangingX?.Invoke(this, e);
    }

    private void NotifySource_PropertyChanged(TSource? sender, PropertyChangedXEventArgs e)
    {
        if (ListeningPropertyNames.Contains(e.PropertyName!))
            ListenedPropertyChangedX?.Invoke(this, e);
    }

    /// <summary>
    /// 监听的属性更改前
    /// </summary>
    public event PropertyChangingXEventHandler<
        PropertyChangeListener<TSource>
    >? ListenedPropertyChangingX;

    /// <summary>
    /// 监听的属性更改后
    /// </summary>
    public event PropertyChangedXEventHandler<
        PropertyChangeListener<TSource>
    >? ListenedPropertyChangedX;
}
