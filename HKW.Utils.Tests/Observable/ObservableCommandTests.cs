using HKW.HKWUtils.Observable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKWTests.Observable;

[TestClass]
public class ObservableCommandTests
{
    [TestMethod]
    public void Execute()
    {
        var triggerEvent = false;
        var command = new ObservableCommand();
        command.ExecuteCommand += () =>
        {
            triggerEvent = true;
        };
        command.Execute(null);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public async Task ExecuteAsync()
    {
        var triggerEvent = false;
        var command = new ObservableCommand();
        command.ExecuteAsyncCommand += async () =>
        {
            await Task.Delay(100);
            triggerEvent = true;
        };
        await command.ExecuteAsync(true);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void IsCanExecute()
    {
        var triggerEvent = false;
        var command = new ObservableCommand();
        command.ExecuteCommand += () =>
        {
            triggerEvent = true;
        };
        command.IsCanExecute = false;
        Assert.IsTrue(command.CanExecute(null) is false);
        command.Execute(null);
        Assert.IsTrue(triggerEvent is false);
    }

    [TestMethod]
    public async Task CurrentCanExecute()
    {
        var triggerEvent = false;
        var command = new ObservableCommand();
        command.ExecuteAsyncCommand += async () =>
        {
            Assert.IsTrue(command.IsCanExecute);
            Assert.IsTrue(command.CurrentCanExecute is false);
            Assert.IsTrue(command.CanExecute(null) is false);
            await Task.Delay(100);
            triggerEvent = true;
        };
        await command.ExecuteAsync(true);
        Assert.IsTrue(command.CurrentCanExecute);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void ExecuteHaveParameter()
    {
        var triggerEvent = false;
        var value = 1;
        var command = new ObservableCommand<int>();
        command.ExecuteCommand += (v) =>
        {
            Assert.IsTrue(v == value);
            triggerEvent = true;
        };
        command.Execute(value);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public async Task ExecuteAsyncHaveParameter()
    {
        var triggerEvent = false;
        var value = 1;
        var command = new ObservableCommand<int>();
        command.ExecuteAsyncCommand += async (v) =>
        {
            Assert.IsTrue(v == value);
            await Task.Delay(100);
            triggerEvent = true;
        };
        await command.ExecuteAsync(value, true);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void IsCanExecuteHaveParameter()
    {
        var triggerEvent = false;
        var value = 1;
        var command = new ObservableCommand<int>();
        command.ExecuteCommand += (v) =>
        {
            Assert.IsTrue(v == value);
            triggerEvent = true;
        };
        command.IsCanExecute = false;
        Assert.IsTrue(command.CanExecute(null) is false);
        command.Execute(value);
        Assert.IsTrue(triggerEvent is false);
    }

    [TestMethod]
    public async Task CurrentCanExecuteHaveParameter()
    {
        var triggerEvent = false;
        var value = 1;
        var command = new ObservableCommand<int>();
        command.ExecuteAsyncCommand += async (v) =>
        {
            Assert.IsTrue(v == value);
            Assert.IsTrue(command.IsCanExecute);
            Assert.IsTrue(command.CurrentCanExecute is false);
            Assert.IsTrue(command.CanExecute(null) is false);
            await Task.Delay(100);
            triggerEvent = true;
        };
        await command.ExecuteAsync(value);
        Assert.IsTrue(command.CurrentCanExecute);
        Assert.IsTrue(triggerEvent);
    }
}
