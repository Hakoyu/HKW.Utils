﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class String
{
    [TestMethod]
    public void FirstLetterCapital()
    {
        string str1 = "red";
        string str2 = str1.FirstLetterCapital();
        Assert.AreEqual(str2, "Red");
    }

    [TestMethod]
    public void FirstLetterCapital_OtherToLower()
    {
        string str1 = "rEd";
        string str2 = str1.FirstLetterCapital();
        Assert.AreEqual(str2, "REd");
        string str3 = str1.FirstLetterCapital(true);
        Assert.AreEqual(str3, "Red");
    }

    [TestMethod]
    public void ToPascal()
    {
        string str1 = "red red red";
        string str2 = str1.ToPascal();
        Assert.AreEqual(str2, "RedRedRed");
    }

    [TestMethod]
    public void ToPascal_NotRemoveSeparator()
    {
        string str1 = "red red red";
        string str2 = str1.ToPascal(removeSeparator: false);
        Assert.AreEqual(str2, "Red Red Red");
    }

    [TestMethod]
    public void ToPascal_OtherToLower()
    {
        string str1 = "rEd rEd rEd";
        string str2 = str1.ToPascal();
        Assert.AreEqual(str2, "REdREdREd");
        string str3 = str1.ToPascal(otherToLower: true);
        Assert.AreEqual(str3, "RedRedRed");
    }
}
