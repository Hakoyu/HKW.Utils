using System.ComponentModel;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 通知属性更改至模型
/// </summary>
/// <typeparam name="T">类型</typeparam>
public class PropertyChangedResponder<T>
    where T : ObservableClass<T>
{
    /// <summary>
    /// 父级
    /// </summary>
    public T Parent { get; private set; }

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; private set; }

    /// <summary>
    /// 通知的属性名称
    /// </summary>
    public HashSet<string> SenderPropertyNames { get; } = new();

    /// <summary>
    /// 通知属性更改至
    /// </summary>
    public event PropertyChangedResponderEventHandler<
        PropertyChangedResponder<T>
    >? SenderPropertyChanged;

    /// <inheritdoc/>
    /// <param name="propertyName">属性名</param>
    /// <param name="observableClass">可观察对象</param>
    public PropertyChangedResponder(string propertyName, T observableClass)
    {
        PropertyName = propertyName;
        Parent = observableClass;
        Parent.PropertyChanged -= ObservableClass_PropertyChanged;
        Parent.PropertyChanged += ObservableClass_PropertyChanged;
    }

    /// <summary>
    /// 关闭
    /// </summary>
    internal void Close()
    {
        Parent.PropertyChanged -= ObservableClass_PropertyChanged;
        Parent = null!;
    }

    private void ObservableClass_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (SenderPropertyNames.Contains(e.PropertyName!))
            SenderPropertyChanged?.Invoke(this, new());
    }
}
