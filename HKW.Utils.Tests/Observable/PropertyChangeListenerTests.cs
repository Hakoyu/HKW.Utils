using HKW.HKWUtils.Observable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKWTests.Observable;

[TestClass]
public class PropertyChangeListenerTests
{
    [TestMethod]
    public void PropertyChanging()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue1 = 1;
        var properties = new HashSet<string>()
        {
            nameof(ObservableClassExample.Value2),
            nameof(ObservableClassExample.Value3)
        };
        var listener = new PropertyChangeListener(example);
        listener.PropertyNames.UnionWith(properties);
        listener.PropertyChanging += (s, e) =>
        {
            if (s is not PropertyChangeListener listener1)
            {
                Assert.Fail();
                return;
            }
            if (listener1.NotifyChangedSource is not ObservableClassExample example1)
            {
                Assert.Fail();
                return;
            }
            // 因为是改变前 所以值不会相等
            Assert.IsTrue(example1.Value2 != newValue1);
            Assert.IsTrue(s!.Equals(listener));
            Assert.IsTrue(s!.Equals(listener));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value2));
            Assert.IsTrue(listener1.PropertyNames.SequenceEqual(properties));
            triggerEvent = true;
        };
        example.Value1 = newValue1;
        // 没有添加Value1 不会触发
        Assert.IsTrue(triggerEvent is false);
        example.Value2 = newValue1;
        Assert.IsTrue(example.Value2 == newValue1);
        Assert.IsTrue(triggerEvent);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var triggerEvent = false;
        var example = new ObservableClassExample();
        var oldValue = example.Value1;
        var newValue1 = 1;
        var properties = new HashSet<string>()
        {
            nameof(ObservableClassExample.Value2),
            nameof(ObservableClassExample.Value3)
        };
        var listener = new PropertyChangeListener(example);
        listener.PropertyNames.UnionWith(properties);
        listener.PropertyChanged += (s, e) =>
        {
            if (s is not PropertyChangeListener listener1)
            {
                Assert.Fail();
                return;
            }
            if (listener1.NotifyChangedSource is not ObservableClassExample example1)
            {
                Assert.Fail();
                return;
            }
            // 因为是改变后 所以值会相等
            Assert.IsTrue(example1.Value2 == newValue1);
            Assert.IsTrue(s!.Equals(listener));
            Assert.IsTrue(e.PropertyName == nameof(ObservableClassExample.Value2));
            Assert.IsTrue(listener1.PropertyNames.SequenceEqual(properties));
            triggerEvent = true;
        };
        example.Value1 = newValue1;
        // 没有添加Value1 不会触发
        Assert.IsTrue(triggerEvent is false);
        example.Value2 = newValue1;
        Assert.IsTrue(example.Value2 == newValue1);
        Assert.IsTrue(triggerEvent);
    }
}
