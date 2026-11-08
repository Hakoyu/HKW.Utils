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
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(getKeyExpression);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(actionsByKey);
        PropertyName = getKeyExpression.GetPropertyName();
        GetKey = getKeyExpression.Compile();
        Source = source;
        Action = action;
        Key = GetKey(source);
        ArgumentNullException.ThrowIfNull(Key);
        _actionsByKey = actionsByKey;
        if (_actionsByKey.TryGetValue(Key, out var actions) is false)
            actions = _actionsByKey[Key] = new();
        actions.Add(this);

        Source.PropertyChanged += Source_PropertyChanged;
    }

    private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_disposed || ReferenceEquals(sender, Source) is false)
            return;
        if (string.IsNullOrEmpty(e.PropertyName) is false && e.PropertyName != PropertyName)
            return;
        CheckWrite();
        _writing = true;
        try
        {
            var newKey = GetKey(Source);
            ArgumentNullException.ThrowIfNull(newKey);
            if (_actionsByKey.Comparer.Equals(Key, newKey))
                return;
            if (_actionsByKey.TryGetValue(newKey, out var newActions) is false)
                newActions = _actionsByKey[newKey] = new();
            newActions.Add(this);
            RemoveCore();
            Key = newKey;
        }
        finally
        {
            _writing = false;
        }
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

    private readonly Dictionary<TKey, List<ValueChangedAction<TKey>>> _actionsByKey;
    private bool _writing;

    private void CheckWrite()
    {
        if (_writing)
            throw new InvalidOperationException(
                "Reentrant writes to the same action registration are not allowed."
            );
    }

    private void RemoveCore()
    {
        if (_actionsByKey.TryGetValue(Key, out var actions))
        {
            actions.Remove(this);
            if (actions.Count == 0)
                _actionsByKey.Remove(Key);
        }
    }

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
            CheckWrite();
            _writing = true;
            try
            {
                Source.PropertyChanged -= Source_PropertyChanged;
                RemoveCore();
            }
            finally
            {
                _writing = false;
            }
        }
        _disposed = true;
    }
    #endregion
}
