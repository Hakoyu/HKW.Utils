using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class EnumInfoTests
{
    //[TestMethod]
    //public void Create()
    //{

    //    foreach (var item in Enum.GetValues<TestEnum>())
    //    {
    //        var info = item.GetInfo();
    //        Assert.AreEqual(item, info.Value);
    //        Assert.AreEqual(item, info);
    //        object.ReferenceEquals(info, item.GetInfo());
    //    }
    //}

    //[TestMethod]
    //public void CreateFlaggable()
    //{
    //    var value = TestFlaggableEnum.A;
    //    foreach (var item in Enum.GetValues<TestFlaggableEnum>())
    //    {
    //        var info = item.GetInfo();
    //        Assert.AreEqual(item, info.Value);
    //        Assert.AreEqual(item, info);
    //        object.ReferenceEquals(info, item.GetInfo());
    //    }
    //}
}

internal enum TestEnum
{
    [Display(Name = "None_Name", ShortName = "None_ShortName", Description = "None_Description")]
    None,

    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 2,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 3,
}

[Flags]
internal enum TestFlaggableEnum
{
    [Display(Name = "None_Name", ShortName = "None_ShortName", Description = "None_Description")]
    None,

    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1 << 0,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 1 << 1,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 1 << 2,
}

internal enum TestEnum_Short : short
{
    None,
    A = 1,
    B = 2,
    C = 3,
}

internal enum TestEnum_Long : long
{
    None,
    A = 1,
    B = 2,
    C = 3,
}
