namespace HKW.HKWUtilsTests;

public static class IReadOnlyListTTestUtils
{
    public static void Test<T>(Func<IList<T>> createList, IReadOnlyCollection<T> newItems)
    {
        IReadOnlyCollectionTTestUtils.Test(createList, newItems);

        ItemGet(createList, newItems);
        ItemSetFail(createList, newItems);

        InsertFail(createList, newItems);
        RemoveAtFail(createList, newItems);

        IndexOf(createList);

        var list = createList();
        if (list is IReadOnlyList<T>)
            Readonly_Test(() => (IReadOnlyList<T>)createList(), newItems);
    }

    public static void ItemGet<T>(Func<IList<T>> createList, IReadOnlyCollection<T> newItems)
    {
        var list = createList();
        var cList = list.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], cList[i]);
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[list.Count]);
    }

    public static void ItemSetFail<T>(Func<IList<T>> createList, IReadOnlyCollection<T> newItems)
    {
        var list = createList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.Throws<NotSupportedException>(() => list[i] = newItems.First());
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1] = newItems.First());
        Assert.Throws<ArgumentOutOfRangeException>(() => list[list.Count] = newItems.First());
    }

    public static void InsertFail<T>(Func<IList<T>> createList, IReadOnlyCollection<T> newItems)
    {
        var list = createList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.Throws<NotSupportedException>(() => list.Insert(i, newItems.First()));
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, newItems.First()));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            list.Insert(list.Count + 1, newItems.First())
        );
    }

    public static void RemoveAtFail<T>(Func<IList<T>> createList, IReadOnlyCollection<T> newItems)
    {
        var list = createList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(i));
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, newItems.First()));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            list.Insert(list.Count + 1, newItems.First())
        );
    }

    public static void IndexOf<T>(Func<IList<T>> createList)
    {
        var list = createList();
        var cList = list.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], cList[i]);
            var item = list[i];
            Assert.AreEqual(list.IndexOf(item), cList.IndexOf(item));
        }
    }

    public static void Readonly_Test<T>(
        Func<IReadOnlyList<T>> createList,
        IReadOnlyCollection<T> newItems
    )
    {
        IReadOnlyCollectionTTestUtils.ReadOnly_Test(createList, newItems);

        ItemGet(createList, newItems);
    }

    public static void ItemGet<T>(
        Func<IReadOnlyList<T>> createList,
        IReadOnlyCollection<T> newItems
    )
    {
        var list = createList();
        var cList = list.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], cList[i]);
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[list.Count]);
    }
}
