﻿using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
public class ReadOnlySetTests
{
    [TestMethod]
    public void ReadOnly()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var readOnlySet = new ReadOnlySet<int>(set);
        Assert.IsTrue(set.SequenceEqual(readOnlySet));
    }
}
