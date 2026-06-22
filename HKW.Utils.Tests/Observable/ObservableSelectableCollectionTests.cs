using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;
using HKW.HKWUtilsTests.Extensions;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableSelectableCollectionTests
{
    [TestMethod]
    public void ObservableSelectableList()
    {
        var list = new ObservableSelectableList<int>();
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);
        IListTTestUtils.Test<int>(
            list,
            Enumerable.Range(1, 10).ToList(),
            () => Random.Shared.Next(100, 1000)
        );

        list = new ObservableSelectableList<int>(Enumerable.Range(1, 10), 0);
        Assert.AreEqual(0, list.SelectedIndex);
        Assert.AreEqual(1, list.SelectedItem);

        list.SelectedItem = 5;
        Assert.AreEqual(4, list.SelectedIndex);
        Assert.AreEqual(5, list.SelectedItem);

        list.Remove(5);
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);

        list.SelectedItem = 4;
        Assert.AreEqual(3, list.SelectedIndex);
        Assert.AreEqual(4, list.SelectedItem);

        list.Insert(3, 11);
        Assert.AreEqual(4, list.SelectedIndex);
        Assert.AreEqual(4, list.SelectedItem);

        list.Add(99);
        Assert.AreEqual(4, list.SelectedIndex);
        Assert.AreEqual(4, list.SelectedItem);

        list.Remove(11);
        Assert.AreEqual(3, list.SelectedIndex);
        Assert.AreEqual(4, list.SelectedItem);

        list.Remove(10);
        Assert.AreEqual(3, list.SelectedIndex);
        Assert.AreEqual(4, list.SelectedItem);

        list[3] = 40;
        Assert.AreEqual(3, list.SelectedIndex);
        Assert.AreEqual(40, list.SelectedItem);

        list.Clear();
        Assert.AreEqual(-1, list.SelectedIndex);
        Assert.AreEqual(default, list.SelectedItem);
    }

    [TestMethod]
    public void ObservableSelectableSet()
    {
        var set = new ObservableSelectableSet<int>();
        Assert.AreEqual(default, set.SelectedItem);
        ISetTestUtils.Test<int>(
            set,
            Enumerable.Range(1, 10).ToHashSet(),
            () => Random.Shared.Next(100, 1000)
        );

        //set = new ObservableSelectableSet<int>(Enumerable.Range(1, 10), 0);
    }
}
