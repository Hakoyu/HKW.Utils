using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[Flags]
public enum TestEnum
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z
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
