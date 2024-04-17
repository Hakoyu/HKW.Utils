using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Collections;

public class IListFindTestUtils
{
    public static void Test(IList<int> list)
    {
        Find(list);
        FindIndex(list);
        FindLast(list);
        FindLastIndex(list);
    }

    public static void Find(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        Assert.IsTrue(list.Find(x => x == 1) == comparisonList.Find(x => x == 1));
        Assert.IsTrue(list.Find(1, x => x == 2) == (1, comparisonList.Find(x => x == 2)));
        Assert.IsTrue(list.Find(1, 3, x => x == 3) == (2, comparisonList.Find(x => x == 3)));

        Assert.IsTrue(list.Find(x => x == -1) == default);
        Assert.IsTrue(list.Find(1, x => x == -1) == (-1, default));
        Assert.IsTrue(list.Find(1, 3, x => x == -1) == (-1, default));

        list.Clear();
    }

    public static void FindIndex(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        Assert.IsTrue(
            comparisonList.FindIndex(x => x == 1) == comparisonList.FindIndex(x => x == 1)
        );
        Assert.IsTrue(
            comparisonList.FindIndex(1, x => x == 2) == comparisonList.FindIndex(x => x == 2)
        );
        Assert.IsTrue(
            comparisonList.FindIndex(1, 3, x => x == 3) == comparisonList.FindIndex(x => x == 3)
        );

        Assert.IsTrue(comparisonList.FindIndex(x => x == -1) == -1);
        Assert.IsTrue(comparisonList.FindIndex(1, x => x == -1) == -1);
        Assert.IsTrue(comparisonList.FindIndex(1, 3, x => x == -1) == -1);

        list.Clear();
    }

    public static void FindLast(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        Assert.IsTrue(comparisonList.FindLast(x => x == 1) == comparisonList.FindLast(x => x == 1));
        Assert.IsTrue(
            comparisonList.FindLast(1, x => x == 2) == (1, comparisonList.FindLast(x => x == 2))
        );
        Assert.IsTrue(
            comparisonList.FindLast(4, 3, x => x == 3) == (2, comparisonList.FindLast(x => x == 3))
        );

        Assert.IsTrue(comparisonList.FindLast(x => x == -1) == default);
        Assert.IsTrue(comparisonList.FindLast(1, x => x == -1) == (-1, default));
        Assert.IsTrue(comparisonList.FindLast(4, 3, x => x == -1) == (-1, default));

        list.Clear();
    }

    public static void FindLastIndex(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        Assert.IsTrue(
            comparisonList.FindLastIndex(x => x == 1) == comparisonList.FindLastIndex(x => x == 1)
        );
        Assert.IsTrue(
            comparisonList.FindLastIndex(1, x => x == 2)
                == comparisonList.FindLastIndex(x => x == 2)
        );
        Assert.IsTrue(
            comparisonList.FindLastIndex(4, 3, x => x == 3)
                == comparisonList.FindLastIndex(x => x == 3)
        );

        Assert.IsTrue(comparisonList.FindLastIndex(x => x == -1) == -1);
        Assert.IsTrue(comparisonList.FindLastIndex(1, x => x == -1) == -1);
        Assert.IsTrue(comparisonList.FindLastIndex(4, 3, x => x == -1) == -1);

        list.Clear();
    }
}
