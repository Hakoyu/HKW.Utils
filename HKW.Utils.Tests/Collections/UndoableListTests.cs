using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

#pragma warning disable S6608

[TestClass]
public class UndoableListTests
{
    static readonly Func<UndoableList<string>> _createList = () =>
        new UndoableList<string>(Enumerable.StringRange(1, 10));

    static readonly IReadOnlyCollection<string> _newItems = Enumerable
        .Range(100, 10)
        .Select(i => i.ToString())
        .ToArray();

    [TestMethod]
    public void ListTest()
    {
        IListTTestUtils.Test(_createList, _newItems);
    }

    [TestMethod]
    public void Undo()
    {
        var list = _createList();
        Assert.IsTrue(list.Undo());
        Assert.HasCount(1, list.UndoStack);
        Assert.AreEqual("10", list.UndoStack.First());
        Assert.AreEqual(9, list.Count);
        Assert.AreEqual("9", list.Last());
    }

    [TestMethod]
    public void UndoFail()
    {
        var list = _createList();
        list.Clear();
        Assert.IsFalse(list.Undo());
    }

    [TestMethod]
    public void UndoCount()
    {
        var list = _createList();
        var cList = list.ToList();
        Assert.IsTrue(list.Undo(5));
        Assert.HasCount(5, list.UndoStack);
        Assert.IsTrue(list.UndoStack.SequenceEqual(cList.Skip(5)));
        Assert.AreEqual(5, list.Count);
        Assert.AreEqual("5", list.Last());
    }

    [TestMethod]
    public void UndoCountFail()
    {
        var list = _createList();
        Assert.IsFalse(list.Undo(-1));
        Assert.IsFalse(list.Undo(list.Count + 1));
        list.Clear();
        Assert.IsFalse(list.Undo(1));
    }

    [TestMethod]
    public void UndoItem()
    {
        var list = _createList();
        var cList = list.ToList();
        Assert.IsTrue(list.Undo("5"));
        Assert.HasCount(4, list);
        Assert.HasCount(6, list.UndoStack);
        Assert.IsTrue(list.UndoStack.SequenceEqual(cList.Skip(cList.IndexOf("5"))));
        Assert.AreEqual("4", list.Last());
    }

    [TestMethod]
    public void UndoItemFail()
    {
        var list = _createList();
        Assert.IsFalse(list.Undo("-1"));
        Assert.IsFalse(list.Undo("100"));
        list.Clear();
        Assert.IsFalse(list.Undo("1"));
    }

    [TestMethod]
    public void Redo()
    {
        var list = _createList();
        Assert.IsTrue(list.Undo());
        Assert.IsTrue(list.Redo());
        Assert.IsEmpty(list.UndoStack);
        Assert.AreEqual(10, list.Count);
        Assert.AreEqual("10", list.Last());
    }

    [TestMethod]
    public void RedoFail()
    {
        var list = _createList();
        Assert.IsFalse(list.Redo());
        Assert.IsEmpty(list.UndoStack);
        Assert.AreEqual(10, list.Count);
        Assert.AreEqual("10", list.Last());
    }

    [TestMethod]
    public void RedoCount()
    {
        var list = _createList();

        for (var i = 1; i <= list.Count; i++)
        {
            Assert.IsTrue(list.Undo(i));
            Assert.HasCount(i, list.UndoStack);
            Assert.IsTrue(list.Redo(i));
            Assert.IsEmpty(list.UndoStack);
            Assert.AreEqual(10, list.Count);
            Assert.AreEqual("10", list.Last());
        }
    }

    [TestMethod]
    public void RedoCountFail()
    {
        var list = _createList();
        Assert.IsFalse(list.Redo(-1));
        Assert.IsFalse(list.Redo(0));
        Assert.IsFalse(list.Redo(1));
        Assert.IsTrue(list.Undo(5));
        Assert.IsFalse(list.Redo(-1));
        Assert.IsFalse(list.Redo(6));
    }

    [TestMethod]
    public void RedoItem()
    {
        var list = _createList();

        for (var i = 1; i <= list.Count; i++)
        {
            Assert.IsTrue(list.Undo(list[i - 1]));
            Assert.IsTrue(list.Redo(list.UndoStack.Last()));
            Assert.IsEmpty(list.UndoStack);
            Assert.AreEqual(10, list.Count);
            Assert.AreEqual("10", list.Last());
        }
    }

    [TestMethod]
    public void RedoItemFail()
    {
        var list = _createList();
        Assert.IsFalse(list.Redo("1"));
        Assert.IsTrue(list.Undo("5"));
        Assert.IsFalse(list.Redo("4"));
        Assert.IsFalse(list.Redo("11"));
    }
}
#pragma warning restore S6608
