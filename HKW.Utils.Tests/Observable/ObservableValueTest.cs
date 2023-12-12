using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableValueTest
{
    [TestMethod]
    public void ValueChanging()
    {
        var trigger = false;
        var ov = new ObservableValue<int>(114);
        ov.ValueChanging += (s, e) =>
        {
            trigger = true;
            Assert.IsTrue(e.OldValue == 114);
            Assert.IsTrue(e.NewValue == 514);
            e.Cancel = false;
        };
        ov.Value = 514;
        Assert.IsTrue(trigger);
        Assert.IsTrue(ov.Value == 514);
    }

    [TestMethod]
    public void ValueChanging_Cancel()
    {
        var trigger = false;
        var ov = new ObservableValue<int>(114);
        ov.ValueChanging += (s, e) =>
        {
            trigger = true;
            Assert.IsTrue(e.OldValue == 114);
            Assert.IsTrue(e.NewValue == 514);
            // cancel
            e.Cancel = true;
        };
        ov.Value = 514;
        Assert.IsTrue(trigger);
        Assert.IsTrue(ov.Value == 114);
    }

    [TestMethod]
    public void ValueChanged()
    {
        var trigger = false;
        var ov = new ObservableValue<int>(114);
        ov.ValueChanged += (s, e) =>
        {
            trigger = true;
            Assert.IsTrue(e.OldValue == 114);
            Assert.IsTrue(e.NewValue == 514);
        };
        ov.Value = 514;
        Assert.IsTrue(trigger);
        Assert.IsTrue(ov.Value == 514);
    }

    [TestMethod]
    public void PropertyChanging()
    {
        var trigger = false;
        var ov = new ObservableValue<int>(114);
        ov.PropertyChanging += (s, e) =>
        {
            trigger = true;
            Assert.IsTrue(s?.Equals(ov));
            Assert.IsTrue(e.PropertyName == nameof(ov.Value));
        };
        ov.Value = 514;
        Assert.IsTrue(trigger);
        Assert.IsTrue(ov.Value == 514);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var trigger = false;
        var ov = new ObservableValue<int>(114);
        ov.PropertyChanged += (s, e) =>
        {
            trigger = true;
            Assert.IsTrue(s?.Equals(ov));
            Assert.IsTrue(e.PropertyName == nameof(ov.Value));
        };
        ov.Value = 514;
        Assert.IsTrue(trigger);
        Assert.IsTrue(ov.Value == 514);
    }
}
