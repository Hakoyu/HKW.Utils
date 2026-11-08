using System;
using System.Collections.Generic;
using System.Text;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtilsTests.Exceptions;

[TestClass]
public class ArgumentExceptionTests
{
    [TestMethod]
    public void ThrowIfNotEquals_1()
    {
        ArgumentException.ThrowIfNotEquals(0, 0);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfNotEquals(0, 1);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_2()
    {
        ArgumentException.ThrowIfAllNotEquals(0, 0, 1);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_3()
    {
        ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_4()
    {
        ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2, 3);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_5()
    {
        ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2, 3, 4);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4, 5);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_Array()
    {
        ArgumentException.ThrowIfAllNotEquals(0, [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
        });
    }
}
