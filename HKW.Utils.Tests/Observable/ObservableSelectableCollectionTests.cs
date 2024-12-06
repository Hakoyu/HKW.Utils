using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;
using HKW.HKWUtils.Tests.Extensions;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableSelectableCollectionTests
{
    [TestMethod]
    public void ObservableSelectableList()
    {
        var list = new ObservableSelectableList<int>();
        Assert.IsTrue(list.SelectedIndex == -1);
        Assert.IsTrue(list.SelectedItem == default);
        IListTestUtils.Test<int>(
            list,
            Enumerable.Range(1, 10).ToList(),
            () => Random.Shared.Next(100, 1000)
        );

        list = new ObservableSelectableList<int>(Enumerable.Range(1, 10), 0);
        Assert.IsTrue(list.SelectedIndex == 0);
        Assert.IsTrue(list.SelectedItem == 1);

        list.SelectedItem = 5;
        Assert.IsTrue(list.SelectedIndex == 4);
        Assert.IsTrue(list.SelectedItem == 5);

        list.Remove(5);
        Assert.IsTrue(list.SelectedIndex == -1);
        Assert.IsTrue(list.SelectedItem == default);

        list.SelectedItem = 4;
        Assert.IsTrue(list.SelectedIndex == 3);
        Assert.IsTrue(list.SelectedItem == 4);

        list.Insert(3, 11);
        Assert.IsTrue(list.SelectedIndex == 4);
        Assert.IsTrue(list.SelectedItem == 4);

        list.Add(99);
        Assert.IsTrue(list.SelectedIndex == 4);
        Assert.IsTrue(list.SelectedItem == 4);

        list.Remove(11);
        Assert.IsTrue(list.SelectedIndex == 3);
        Assert.IsTrue(list.SelectedItem == 4);

        list.Remove(10);
        Assert.IsTrue(list.SelectedIndex == 3);
        Assert.IsTrue(list.SelectedItem == 4);

        list[3] = 40;
        Assert.IsTrue(list.SelectedIndex == 3);
        Assert.IsTrue(list.SelectedItem == 40);

        list.Clear();
        Assert.IsTrue(list.SelectedIndex == -1);
        Assert.IsTrue(list.SelectedItem == default);
    }

    [TestMethod]
    public void ObservableSelectableSet()
    {
        var set = new ObservableSelectableSet<int>();
        Assert.IsTrue(set.SelectedItem == default);
        ISetTestUtils.Test<int>(
            set,
            Enumerable.Range(1, 10).ToHashSet(),
            () => Random.Shared.Next(100, 1000)
        );

        //set = new ObservableSelectableSet<int>(Enumerable.Range(1, 10), 0);
    }
}
