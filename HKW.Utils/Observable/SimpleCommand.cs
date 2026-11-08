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

    /// <inheritdoc/>
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

    /// <summary>
    /// 查询是否可执行, 不检查或转换参数.
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    /// <summary>
    /// 直接转换参数并执行, 由调用方决定是否先检查 CanExecute.
    /// </summary>
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
