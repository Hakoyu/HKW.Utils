﻿using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class ISetTests
{
    [TestMethod]
    public void AsReadOnly()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var readOnlySet = set.AsReadOnly();
        Assert.IsTrue(set.SequenceEqual(readOnlySet));
    }
}
