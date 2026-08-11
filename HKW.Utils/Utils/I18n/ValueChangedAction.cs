using System.ComponentModel;
using System.Linq.Expressions;
using HKW.HKWUtils.Extensions;

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
    /// <param name="actionsByKey">行动字典</param>
    public ValueChangedAction(
        INotifyPropertyChanged source,
        Expression<Func<INotifyPropertyChanged, TKey>> getKeyExpression,
        ValueChangedActionHandler<TKey> action,
        Dictionary<TKey, List<ValueChangedAction<TKey>>> actionsByKey
    )
    {
        PropertyName = getKeyExpression.GetPropertyName();
        GetKey = getKeyExpression.Compile();
        Source = source;
        Action = action;
        Key = GetKey(source);
        _actionsByKey = actionsByKey;
        if (_actionsByKey.TryGetValue(Key, out var actions) is false)
            actions = _actionsByKey[Key] = new();
        actions.Add(this);

        Source.PropertyChanged += Source_PropertyChanged;
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != PropertyName)
            return;
        var newKey = GetKey(Source);
        ArgumentNullException.ThrowIfNull(newKey);
        if (_actionsByKey.TryGetValue(Key, out var oldActions))
        {
            oldActions.Remove(this);
            if (oldActions.Count == 0)
                _actionsByKey.Remove(Key);
        }

        Key = newKey!;
        if (_actionsByKey.TryGetValue(newKey!, out var newActions) is false)
            newActions = _actionsByKey[newKey!] = new();
        newActions.Add(this);
    }

    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; private set; }

    /// <summary>
    /// 获取键
    /// </summary>
    public Func<INotifyPropertyChanged, TKey> GetKey { get; private set; }

    /// <summary>
    /// 源
    /// </summary>
    public INotifyPropertyChanged Source { get; private set; }

    /// <summary>
    /// 值改变行动
    /// </summary>
    public ValueChangedActionHandler<TKey> Action { get; private set; }

    private Dictionary<TKey, List<ValueChangedAction<TKey>>> _actionsByKey;

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

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            if (_actionsByKey.TryGetValue(Key, out var actions))
            {
                actions.Remove(this);
                if (actions.Count == 0)
                    _actionsByKey.Remove(Key);
            }
            _actionsByKey = null!;
            Source = null!;
            GetKey = null!;
            Action = null!;
        }
        _disposed = true;
    }
    #endregion
}
