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
        try
        {
            ArgumentException.ThrowIfNotEquals(0, 0);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfNotEquals(0, 1);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_2()
    {
        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 0, 1);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_3()
    {
        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_4()
    {
        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2, 3);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_5()
    {
        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 0, 1, 2, 3, 4);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4, 5);
        });
    }

    [TestMethod]
    public void ThrowIfAllNotEquals_Array()
    {
        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
        }
        catch
        {
            Assert.Fail();
        }

        Assert.Throws<ArgumentException>(() =>
        {
            ArgumentException.ThrowIfAllNotEquals(0, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
        });
    }
}
