using System.Collections.ObjectModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;
using HKW.HKWUtilsTests.Extensions;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableSelectableCollectionTests
{
    [TestMethod]
    public void ObservableSelectableList_SelectionTest()
    {
        var list = new ObservableSelectableList<string>(["1", "2", "3"]);

        Assert.IsFalse(list.HasSelection);
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);

        list.SelectedIndex = 1;

        Assert.IsTrue(list.HasSelection);
        Assert.AreEqual(1, list.SelectedIndex);
        Assert.AreEqual("2", list.SelectedItem);

        list.Insert(0, "0");

        Assert.IsTrue(list.HasSelection);
        Assert.AreEqual(2, list.SelectedIndex);
        Assert.AreEqual("2", list.SelectedItem);

        list.Remove("0");

        Assert.IsTrue(list.HasSelection);
        Assert.AreEqual(1, list.SelectedIndex);
        Assert.AreEqual("2", list.SelectedItem);

        list[1] = "20";

        Assert.IsTrue(list.HasSelection);
        Assert.AreEqual(1, list.SelectedIndex);
        Assert.AreEqual("20", list.SelectedItem);

        list.RemoveAt(1);

        Assert.IsFalse(list.HasSelection);
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableList_SelectedItemTest()
    {
        var list = new ObservableSelectableList<string>(["1", "2", "3"]);

        list.SelectedItem = "3";

        Assert.IsTrue(list.HasSelection);
        Assert.AreEqual(2, list.SelectedIndex);
        Assert.AreEqual("3", list.SelectedItem);

        list.SelectedItem = "4";

        Assert.IsFalse(list.HasSelection);
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);

        Assert.Throws<ArgumentOutOfRangeException>(() => list.SelectedIndex = list.Count);
    }

    [TestMethod]
    public void ObservableSelectableList_ClearSelectionTest()
    {
        var list = new ObservableSelectableList<string>(["1", "2", "3"]);
        list.SelectedIndex = 1;

        list.Clear();

        Assert.IsFalse(list.HasSelection);
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableSet_SelectionTest()
    {
        var set = new ObservableSelectableSet<string>(["1", "2", "3"]);

        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(default, set.SelectedItem);

        set.SelectedItem = "2";

        Assert.IsTrue(set.HasSelection);
        Assert.AreEqual("2", set.SelectedItem);
        Assert.AreEqual(1, set.SelectedIndex);

        Assert.IsTrue(set.Remove("2"));

        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(default, set.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableSet_SelectedIndexTest()
    {
        var set = new ObservableSelectableSet<string>(["1", "2", "3"]);

        set.SelectedIndex = 2;

        Assert.IsTrue(set.HasSelection);
        Assert.AreEqual(2, set.SelectedIndex);
        Assert.AreEqual("3", set.SelectedItem);

        set.SelectedIndex = -1;

        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(default, set.SelectedItem);

        Assert.Throws<ArgumentOutOfRangeException>(() => set.SelectedIndex = set.Count);
    }

    [TestMethod]
    public void ObservableSelectableSet_OperationClearsSelectionTest()
    {
        var set = new ObservableSelectableSet<string>(["1", "2", "3"]);
        set.SelectedItem = "2";

        set.ExceptWith(["2"]);

        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(default, set.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableSet_ClearSelectionTest()
    {
        var set = new ObservableSelectableSet<string>(["1", "2", "3"]);
        set.SelectedItem = "2";

        set.Clear();

        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(default, set.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableDictionary_SelectionTest()
    {
        var dictionary = new ObservableSelectableDictionary<string, string>([
            new("1", "Value1"),
            new("2", "Value2"),
            new("3", "Value3"),
        ]);

        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(-1, dictionary.SelectedIndex);
        Assert.AreEqual(default(KeyValuePair<string, string>), dictionary.SelectedItem);

        dictionary.SelectedKey = "2";

        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreEqual(1, dictionary.SelectedIndex);
        Assert.AreEqual("2", dictionary.SelectedKey);
        Assert.AreEqual("Value2", dictionary.SelectedValue);
        Assert.AreEqual(new KeyValuePair<string, string>("2", "Value2"), dictionary.SelectedItem);

        dictionary["2"] = "NewValue2";

        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreEqual(1, dictionary.SelectedIndex);
        Assert.AreEqual("2", dictionary.SelectedKey);
        Assert.AreEqual("NewValue2", dictionary.SelectedValue);
        Assert.AreEqual(
            new KeyValuePair<string, string>("2", "NewValue2"),
            dictionary.SelectedItem
        );

        Assert.IsTrue(dictionary.Remove("2"));

        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(-1, dictionary.SelectedIndex);
        Assert.AreEqual(default(KeyValuePair<string, string>), dictionary.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableDictionary_SelectedIndexTest()
    {
        var dictionary = new ObservableSelectableDictionary<string, string>([
            new("1", "Value1"),
            new("2", "Value2"),
            new("3", "Value3"),
        ]);

        dictionary.SelectedIndex = 2;

        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreEqual(2, dictionary.SelectedIndex);
        Assert.AreEqual("3", dictionary.SelectedKey);
        Assert.AreEqual("Value3", dictionary.SelectedValue);

        dictionary.SelectedIndex = -1;

        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(-1, dictionary.SelectedIndex);
        Assert.AreEqual(default(KeyValuePair<string, string>), dictionary.SelectedItem);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            dictionary.SelectedIndex = dictionary.Count
        );
    }

    [TestMethod]
    public void ObservableSelectableDictionary_ClearSelectionTest()
    {
        var dictionary = new ObservableSelectableDictionary<string, string>([
            new("1", "Value1"),
            new("2", "Value2"),
        ]);
        dictionary.SelectedKey = "1";

        dictionary.Clear();

        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(-1, dictionary.SelectedIndex);
        Assert.AreEqual(default(KeyValuePair<string, string>), dictionary.SelectedItem);
    }
}
