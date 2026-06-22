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

        try
        {
            ArgumentException.ThrowIfNotEquals(0, 1);
            Assert.Fail();
        }
        catch { }
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

        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2);
            Assert.Fail();
        }
        catch { }
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

        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3);
            Assert.Fail();
        }
        catch { }
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

        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4);
            Assert.Fail();
        }
        catch { }
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

        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, 1, 2, 3, 4, 5);
            Assert.Fail();
        }
        catch { }
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

        try
        {
            ArgumentException.ThrowIfAllNotEquals(0, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
            Assert.Fail();
        }
        catch { }
    }
}
