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
        Assert.IsTrue(list.UndoStack.Count == 1);
        Assert.IsTrue(list.UndoStack.First() == 9);
        Assert.IsTrue(list.Count == 9);
        Assert.IsTrue(list.Last() == 8);
    }

    [TestMethod]
    public void Undo2()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo(5);
        Assert.IsTrue(list.UndoStack.Count == 5);
        Assert.IsTrue(list.UndoStack.SequenceEqual(Enumerable.Range(5, 5)));
        Assert.IsTrue(list.Count == 5);
        Assert.IsTrue(list.Last() == 4);
    }

    [TestMethod]
    public void Redo1()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo();
        list.Redo();
        Assert.IsTrue(list.UndoStack.Count == 0);
        Assert.IsTrue(list.Count == 10);
        Assert.IsTrue(list.Last() == 9);
    }

    [TestMethod]
    public void Redo2()
    {
        var list = new UndoableList<int>(Enumerable.Range(0, 10));
        list.Undo(5);
        list.Redo(5);
        Assert.IsTrue(list.UndoStack.Count == 0);
        Assert.IsTrue(list.Count == 10);
        Assert.IsTrue(list.Last() == 9);
        list.Undo(5);
        list.Redo(3);
        Assert.IsTrue(list.UndoStack.Count == 2);
        Assert.IsTrue(list.UndoStack.SequenceEqual(Enumerable.Range(8, 2)));
        Assert.IsTrue(list.Count == 8);
        Assert.IsTrue(list.Last() == 7);
    }
}
