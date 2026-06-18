using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
public class UndoableListTests
{
    [TestMethod]
    public void ListTest()
    {
        IListTTestUtils.Test(
            new UndoableList<int>(),
            Enumerable.Range(0, 10).ToList(),
            () => Random.Shared.Next(10)
        );
    }

    [TestMethod]
    public void Undo1()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo();
        Assert.HasCount(1, list.UndoStack);
        Assert.AreEqual(9, list.UndoStack.First());
        Assert.AreEqual(9, list.Count);
        Assert.AreEqual(8, list.Last());
    }

    [TestMethod]
    public void Undo2()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo(5);
        Assert.HasCount(5, list.UndoStack);
        Assert.IsTrue(list.UndoStack.SequenceEqual(Enumerable.Range(5, 5)));
        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(4, list.Last());
    }

    [TestMethod]
    public void Redo1()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo();
        list.Redo();
        Assert.IsEmpty(list.UndoStack);
        Assert.AreEqual(10, list.Count);
        Assert.AreEqual(9, list.Last());
    }

    [TestMethod]
    public void Redo2()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo(5);
        list.Redo(5);
        Assert.IsEmpty(list.UndoStack);
        Assert.AreEqual(10, list.Count);
        Assert.AreEqual(9, list.Last());
        list.Undo(5);
        list.Redo(3);
        Assert.HasCount(2, list.UndoStack);
        Assert.IsTrue(list.UndoStack.SequenceEqual(Enumerable.Range(8, 2)));
        Assert.AreEqual(8, list.Count);
        Assert.AreEqual(7, list.Last());
    }
}
