using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 简易命令
/// </summary>
public sealed class SimpleCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// 初始化简易命令
    /// </summary>
    /// <param name="execute">执行方法</param>
    /// <param name="canExecute">是否可执行的判断方法</param>
    public SimpleCommand(Action<T> execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);

        _execute = execute;
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        _execute((T)parameter!);
    }

    /// <summary>
    /// 通知命令可执行状态已更改
    /// </summary>
    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
