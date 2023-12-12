using HKW.HKWUtils.Observable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableClassTests
{
    [TestMethod]
    public void PropertyChanging()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue = 1;
        example.PropertyChanging += (s, e) =>
        {
            if (s is not ObservableClassExample oc)
            {
                Assert.Fail();
                return;
            }
            Assert.IsTrue(s!.Equals(example));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value1));
            Assert.IsTrue(oc.Value1 == oldValue);
            triggerEvent = true;
        };
        example.Value1 = newValue;
        Assert.IsTrue(example.Value1 == newValue);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue = 1;
        example.PropertyChanged += (s, e) =>
        {
            if (s is not ObservableClassExample oc)
            {
                Assert.Fail();
                return;
            }
            Assert.IsTrue(s!.Equals(example));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value1));
            Assert.IsTrue(oc.Value1 == newValue);
            triggerEvent = true;
        };
        example.Value1 = newValue;
        Assert.IsTrue(example.Value1 == newValue);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void PropertyChangingX()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue = 1;
        example.PropertyChangingX += (s, e) =>
        {
            Assert.IsTrue(s!.Equals(example));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value1));
            Assert.IsTrue(s.Value1 != newValue);
            Assert.IsTrue(e.OldValue!.Equals(oldValue));
            Assert.IsTrue(e.NewValue!.Equals(newValue));
            triggerEvent = true;
        };
        example.Value1 = newValue;
        Assert.IsTrue(example.Value1 == newValue);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void PropertyChangingX_Cancel()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue = 1;
        example.PropertyChangingX += (s, e) =>
        {
            Assert.IsTrue(s!.Equals(example));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value1));
            Assert.IsTrue(s.Value1 != newValue);
            Assert.IsTrue(e.OldValue!.Equals(oldValue));
            Assert.IsTrue(e.NewValue!.Equals(newValue));
            triggerEvent = true;
            e.Cancel = true;
        };
        example.Value1 = newValue;
        Assert.IsTrue(example.Value1 == oldValue);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void PropertyChangedX()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue = 1;
        example.PropertyChangedX += (s, e) =>
        {
            Assert.IsTrue(s!.Equals(example));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value1));
            Assert.IsTrue(s.Value1 == newValue);
            Assert.IsTrue(e.OldValue!.Equals(oldValue));
            Assert.IsTrue(e.NewValue!.Equals(newValue));
            triggerEvent = true;
        };
        example.Value1 = newValue;
        Assert.IsTrue(example.Value1 == newValue);
        Assert.IsTrue(triggerEvent);
    }
}

public class ObservableClassExample : ObservableClass<ObservableClassExample>
{
    int _value1 = 0;
    public int Value1
    {
        get => _value1;
        set => SetProperty(ref _value1, value);
    }
    int _value2 = 0;
    public int Value2
    {
        get => _value2;
        set => SetProperty(ref _value2, value);
    }

    int _value3 = 0;
    public int Value3
    {
        get => _value3;
        set => SetProperty(ref _value3, value);
    }
}
