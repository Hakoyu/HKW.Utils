using System.Linq.Expressions;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class ExpressionTests
{
    [TestMethod]
    public void PropertyAccess_AndBuiltInConversions_AreAccepted()
    {
        Expression<Func<Model, int>> direct = model => model.Value;
        Expression<Func<Model, object>> boxed = model => model.Value;
        Expression<Func<Model, short>> checkedCast = model => checked((short)model.Value);
        Assert.AreEqual("Value", direct.GetPropertyName());
        Assert.AreEqual("Value", boxed.GetPropertyName());
        Assert.AreEqual("Value", checkedCast.GetPropertyName());
    }

    [TestMethod]
    public void Fields_AndUnaryOperations_AreRejected()
    {
        Expression<Func<Model, int>> field = model => model.Field;
        Expression<Func<Model, int>> negated = model => -model.Value;
        Expression<Func<Model, int>> added = model => model.Value + 1;
        foreach (LambdaExpression expression in new LambdaExpression[] { field, negated, added })
        {
            var error = Assert.ThrowsExactly<ArgumentException>(() => expression.GetPropertyName());
            Assert.AreEqual("expression", error.ParamName);
        }
    }

    [TestMethod]
    public void NullExpression_IsRejected()
    {
        var error = Assert.ThrowsExactly<ArgumentNullException>(() =>
            ((LambdaExpression)null!).GetPropertyName()
        );
        Assert.AreEqual("expression", error.ParamName);
    }

    private sealed class Model
    {
        public int Field = 1;
        public int Value { get; set; }
    }
}
