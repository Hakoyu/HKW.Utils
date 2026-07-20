using System.Collections.ObjectModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;
using HKW.HKWUtilsTests.Extensions;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableSelectableCollectionTests
{
    //static Func<ObservableSelectableListWrapper<string>> _createList = () =>
    //    new ObservableSelectableList<string>(Enumerable.Range(1, 10).Select(i => i.ToString()));

    //static IReadOnlyCollection<string> _newItems = new ReadOnlyCollection<string>(
    //    Enumerable.Range(100, 10).Select(i => i.ToString()).ToList()
    //);

    //[TestMethod]
    //public void IListTest()
    //{
    //    IListTTestUtils.Test(_createList, _newItems);
    //}

    ////[TestMethod]
    ////public void ObservableListTest()
    ////{
    ////    ObservableCollectionUtils.Test(_createList, _newItems);
    ////}

    //[TestMethod]
    //public void Test()
    //{
    //    var list = new ObservableSelectableList<int>();

    //    Assert.AreEqual(-1, list.SelectedIndex);
    //    Assert.AreEqual(default, list.SelectedItem);

    //    list = new ObservableSelectableList<int>(Enumerable.Range(1, 10));
    //    list.SelectedIndex = 0;
    //    Assert.AreEqual(0, list.SelectedIndex);
    //    Assert.AreEqual(1, list.SelectedItem);

    //    list.SelectedItem = 5;
    //    Assert.AreEqual(4, list.SelectedIndex);
    //    Assert.AreEqual(5, list.SelectedItem);

    //    list.Remove(5);
    //    Assert.AreEqual(-1, list.SelectedIndex);
    //    Assert.AreEqual(default, list.SelectedItem);

    //    list.SelectedItem = 4;
    //    Assert.AreEqual(3, list.SelectedIndex);
    //    Assert.AreEqual(4, list.SelectedItem);

    //    list.Insert(3, 11);
    //    Assert.AreEqual(4, list.SelectedIndex);
    //    Assert.AreEqual(4, list.SelectedItem);

    //    list.Add(99);
    //    Assert.AreEqual(4, list.SelectedIndex);
    //    Assert.AreEqual(4, list.SelectedItem);

    //    list.Remove(11);
    //    Assert.AreEqual(3, list.SelectedIndex);
    //    Assert.AreEqual(4, list.SelectedItem);

    //    list.Remove(10);
    //    Assert.AreEqual(3, list.SelectedIndex);
    //    Assert.AreEqual(4, list.SelectedItem);

    //    list[3] = 40;
    //    Assert.AreEqual(3, list.SelectedIndex);
    //    Assert.AreEqual(40, list.SelectedItem);

    //    list.Clear();
    //    Assert.AreEqual(-1, list.SelectedIndex);
    //    Assert.AreEqual(default, list.SelectedItem);
    //}
}
