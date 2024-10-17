using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[Flags]
internal enum TestEnum
{
    [Display(Name = "A_Name", ShortName = "A_ShortName", Description = "A_Description")]
    A = 1 << 0,

    [Display(Name = "B_Name", ShortName = "B_ShortName", Description = "B_Description")]
    B = 1 << 1,

    [Display(Name = "C_Name", ShortName = "C_ShortName", Description = "C_Description")]
    C = 1 << 2,
}

[TestClass]
public class EnumTests
{
    [TestMethod]
    public void AddFlag()
    {
        Assert.IsTrue(TestEnum.A.AddFlag(TestEnum.B) == (TestEnum.A | TestEnum.B));
    }

    [TestMethod]
    public void RemoveFlag()
    {
        Assert.IsTrue(TestEnum.A.RemoveFlag(TestEnum.B) == (TestEnum.A & ~TestEnum.B));
    }
}
