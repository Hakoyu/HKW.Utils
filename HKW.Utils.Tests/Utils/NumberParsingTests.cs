using System.Globalization;
using System.Numerics;
using System.Text;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class NumberParsingTests
{
    public static IEnumerable<object[]> Types => NumberConversionTests.Types;

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void TextFormsAndEntryPoints_MatchNativeParsing(Type target) =>
        NumberTestData.Run(typeof(NumberParsingTests), nameof(CheckParsing), target);

    private static object Result(bool success, object value) => (success, value.GetType(), value);

    private static void CheckParsing<T>()
        where T : struct, INumber<T>
    {
        var type = NumberUtils.GetNumberType(typeof(T));
        var provider = CultureInfo.InvariantCulture;
        string?[] samples = ["123", "-12", "invalid", "", null, "NaN", "Infinity",
            "340282366920938463463374607431768211456", new string('9', 400)];
        foreach (var text in samples)
        {
            var utf8 = Encoding.UTF8.GetBytes(text ?? "");
            NumberTestData.Check(() => T.Parse(text.AsSpan(), NumberStyles.Any, provider),
                () => NumberUtils.Parse<T>(text!, NumberStyles.Any, provider),
                () => NumberUtils.Parse(typeof(T), text!, NumberStyles.Any, provider),
                () => NumberUtils.Parse(type, text!, NumberStyles.Any, provider));
            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), NumberStyles.Any, provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text, NumberStyles.Any, provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text, NumberStyles.Any, provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, text, NumberStyles.Any, provider, out var value), value));
            NumberTestData.Check(() => T.Parse(text.AsSpan(), provider),
                () => NumberUtils.Parse<T>(text!, provider),
                () => NumberUtils.Parse(typeof(T), text!, provider),
                () => NumberUtils.Parse(type, text!, provider));
            NumberTestData.Check(() => T.Parse(text.AsSpan(), null),
                () => NumberUtils.Parse<T>(text!),
                () => NumberUtils.Parse(typeof(T), text!),
                () => NumberUtils.Parse(type, text!));
            NumberTestData.Check(() => T.Parse(text.AsSpan(), provider),
                () => NumberUtils.Parse<T>(text.AsSpan(), provider),
                () => NumberUtils.Parse(typeof(T), text.AsSpan(), provider),
                () => NumberUtils.Parse(type, text.AsSpan(), provider),
                () => NumberUtils.ConvertTo<T>(text.AsSpan(), provider: provider),
                () => NumberUtils.ConvertTo(text.AsSpan(), typeof(T), provider: provider),
                () => NumberUtils.ConvertTo(text.AsSpan(), type, provider: provider));
            NumberTestData.Check(() => T.Parse(text.AsSpan(), null),
                () => NumberUtils.Parse<T>(text.AsSpan()),
                () => NumberUtils.Parse(typeof(T), text.AsSpan()),
                () => NumberUtils.Parse(type, text.AsSpan()));
            NumberTestData.Check(() => T.Parse(utf8, provider),
                () => NumberUtils.Parse<T>(utf8, provider),
                () => NumberUtils.Parse(typeof(T), utf8, provider),
                () => NumberUtils.Parse(type, utf8, provider));
            NumberTestData.Check(() => T.Parse(utf8, null),
                () => NumberUtils.Parse<T>(utf8),
                () => NumberUtils.Parse(typeof(T), utf8),
                () => NumberUtils.Parse(type, utf8));

            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text, provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text, provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, text, provider, out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), null, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text, out var value), value),
                () => Result(NumberUtils.TryParse(type, text, out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text.AsSpan(), provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text.AsSpan(), provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, text.AsSpan(), provider, out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), null, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text.AsSpan(), out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text.AsSpan(), out var value), value),
                () => Result(NumberUtils.TryParse(type, text.AsSpan(), out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(utf8, provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(utf8, provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), utf8, provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, utf8, provider, out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(utf8, null, out var value), value),
                () => Result(NumberUtils.TryParse<T>(utf8, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), utf8, out var value), value),
                () => Result(NumberUtils.TryParse(type, utf8, out var value), value));
        }
    }

    [TestMethod]
    [DynamicData(nameof(Types))]
    public void StylesAndCulture_MatchNativeParsing(Type target) =>
        NumberTestData.Run(typeof(NumberParsingTests), nameof(CheckStyles), target);

    private static void CheckStyles<T>()
        where T : struct, INumber<T>
    {
        var type = NumberUtils.GetNumberType(typeof(T));
        var provider = CultureInfo.GetCultureInfo("fr-FR");
        foreach (var (text, style) in new[] {
            ("1,5", NumberStyles.Float), ("1,0", NumberStyles.Number),
            ("FF", NumberStyles.HexNumber), ("101", NumberStyles.BinaryNumber),
            ("12", (NumberStyles)(-1)), ("invalid", NumberStyles.Integer) })
        {
            var utf8 = Encoding.UTF8.GetBytes(text);
            NumberTestData.Check(() => T.Parse(text.AsSpan(), style, provider),
                () => NumberUtils.Parse<T>(text.AsSpan(), style, provider),
                () => NumberUtils.Parse(typeof(T), text.AsSpan(), style, provider),
                () => NumberUtils.Parse(type, text.AsSpan(), style, provider),
                () => NumberUtils.ConvertTo<T>(text.AsSpan(), style, provider),
                () => NumberUtils.ConvertTo(text.AsSpan(), typeof(T), style, provider),
                () => NumberUtils.ConvertTo(text.AsSpan(), type, style, provider));
            NumberTestData.Check(() => T.Parse(utf8, style, provider),
                () => NumberUtils.Parse<T>(utf8, style, provider),
                () => NumberUtils.Parse(typeof(T), utf8, style, provider),
                () => NumberUtils.Parse(type, utf8, style, provider));
            NumberTestData.Check(() => Result(T.TryParse(text.AsSpan(), style, provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(text.AsSpan(), style, provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), text.AsSpan(), style, provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, text.AsSpan(), style, provider, out var value), value));
            NumberTestData.Check(() => Result(T.TryParse(utf8, style, provider, out var value), value),
                () => Result(NumberUtils.TryParse<T>(utf8, style, provider, out var value), value),
                () => Result(NumberUtils.TryParse(typeof(T), utf8, style, provider, out var value), value),
                () => Result(NumberUtils.TryParse(type, utf8, style, provider, out var value), value));
        }
    }

    [TestMethod]
    public void ParsingOverloads_UseSpansAndConsistentParameterNames()
    {
        var methods = typeof(NumberUtils).GetMethods();
        foreach (var methodName in new[] { nameof(NumberUtils.Parse), nameof(NumberUtils.TryParse) })
        {
            var overloads = methods.Where(method => method.Name == methodName).ToArray();
            Assert.AreEqual(18, overloads.Length, methodName);
            foreach (var method in overloads)
            {
                var parameters = method.GetParameters();
                Assert.IsFalse(parameters.Any(parameter => parameter.ParameterType == typeof(string)), method.ToString());
                Assert.AreEqual(1, parameters.Count(parameter =>
                    parameter.ParameterType == typeof(ReadOnlySpan<char>) ||
                    parameter.ParameterType == typeof(ReadOnlySpan<byte>)), method.ToString());
            }
        }

        foreach (var method in methods)
        {
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof(ReadOnlySpan<char>))
                    Assert.AreEqual("text", parameter.Name, method.ToString());
                else if (parameter.ParameterType == typeof(ReadOnlySpan<byte>))
                    Assert.AreEqual("utf8Text", parameter.Name, method.ToString());
            }
        }
    }

    [TestMethod]
    public void NamedTextArguments_AreAccepted()
    {
        var utf8 = Encoding.UTF8.GetBytes("123");
        Assert.AreEqual(123, NumberUtils.Parse<int>(text: "123"));
        Assert.AreEqual(123, NumberUtils.Parse<int>(utf8Text: utf8));
        Assert.IsTrue(NumberUtils.TryParse<int>(text: "123", result: out var textResult));
        Assert.IsTrue(NumberUtils.TryParse<int>(utf8Text: utf8, result: out var utf8Result));
        Assert.AreEqual(123, textResult);
        Assert.AreEqual(123, utf8Result);
        Assert.AreEqual(123, NumberUtils.ConvertTo<int>(text: "123"));
    }

    [TestMethod]
    public void UnknownRuntimeTypeAndEnum_AreRejected()
    {
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.Parse((NumberType)999, "1"));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.TryParse((NumberType)999, "1", out _));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.Parse(typeof(bool), "1"));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.TryParse(typeof(bool), "1", out _));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.ConvertTo("1".AsSpan(), (NumberType)999));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.ConvertTo("1".AsSpan(), typeof(bool)));
        Assert.ThrowsExactly<NotImplementedException>(() => NumberUtils.ConvertTo("1".AsSpan(), (Type)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => NumberUtils.Parse((Type)null!, "1"));
        Assert.ThrowsExactly<ArgumentNullException>(() => NumberUtils.TryParse((Type)null!, "1", out _));
    }
}
