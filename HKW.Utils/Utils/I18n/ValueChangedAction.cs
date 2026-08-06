using System.ComponentModel;
using System.Linq.Expressions;

namespace HKW.HKWUtils;

/// <summary>
/// 值改变行动
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
public sealed class ValueChangedAction<TKey> : IDisposable
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="source">源</param>
    /// <param name="getKeyExpression">获取键表达式</param>
    /// <param name="action">行动</param>
    public ValueChangedAction(
        INotifyPropertyChanged source,
        Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
        ValueChangedActionHandler<TKey> action
    )
    {
        PropertyName = getKeyExpression.GetPropertyName();
        GetKey = getKeyExpression.Compile();
        Source = source;
        Action = action;
        Key = GetKey(source);
    }

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; internal set; }

    /// <summary>
    /// 获取键
    /// </summary>
    public Func<INotifyPropertyChanged, TKey> GetKey { get; }

    /// <summary>
    /// 源
    /// </summary>
    public INotifyPropertyChanged Source { get; }

    /// <summary>
    /// 值改变行动
    /// </summary>
    public ValueChangedActionHandler<TKey> Action { get; }

    /// <summary>
    /// 释放器
    /// </summary>
    public IDisposable? Disposable { get; internal set; }

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ValueChangedAction() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
            Disposable?.Dispose();
        _disposed = true;
    }
    #endregion
}
