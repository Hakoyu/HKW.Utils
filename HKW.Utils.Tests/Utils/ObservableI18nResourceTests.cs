using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class ObservableI18nResourceTests
{
    public static CultureInfo[] Cultures { get; } =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures);
    public static CultureInfo CurrentCulture { get; } = CultureInfo.CurrentCulture;

    public static Func<ObservableI18nResource<string, string>> GetI18nResource =>
        () =>
        {
            var i18nResource = new ObservableI18nResource<string, string>(
                "Main",
                Cultures,
                Cultures.First()
            );
            foreach (var c in i18nResource.Cultures)
                i18nResource.SetDatas(
                    Enumerable
                        .Range(0, 10)
                        .Select(x => KeyValuePair.Create(x.ToString(), GetValue(x.ToString(), c))),
                    c
                );

            return i18nResource;
        };
    public static string[] Keys { get; } = Enumerable.StringRange(0, 10).ToArray();

    public static string GetValue(string key, CultureInfo culture) => $"{key}_{culture.Name}";

    [TestMethod]
    public void ChangeCurrentCulture()
    {
        var resource = GetI18nResource();
        foreach (var culture in resource.Cultures)
        {
            resource.CurrentCulture = culture;
            foreach (var key in Keys)
            {
                Assert.AreEqual(
                    GetValue(key, culture),
                    [resource.GetData(key), resource.GetData(key, culture)]
                );
            }
        }
    }

    [TestMethod]
    public void SetData()
    {
        var resource = new ObservableI18nResource<string, string>(
            "Main",
            Cultures,
            Cultures.First()
        );
        foreach (var (c, i) in resource.Cultures.WithIndex())
        {
            Assert.IsTrue(resource.SetData(i.ToString(), GetValue(i.ToString(), c), c));
            Parallel.For(
                0,
                resource.Cultures.Count,
                j =>
                {
                    var key = j.ToString();
                    if (j == i)
                    {
                        // 同Key的值相等
                        Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                    }
                    else if (j < i)
                    {
                        // 小于i代表此key被添加过, 但是默认值
                        Assert.AreEqual(
                            resource.GetData(key, c),
                            resource.GetDefaultValue("", null!)
                        );
                    }
                    else
                    {
                        // 未被添加的key抛出异常
                        Assert.Throws<KeyNotFoundException>(() => resource.GetData(key, c));
                    }
                }
            );
        }
    }

    [TestMethod]
    public void SetDataWhenDefault()
    {
        var resource = new ObservableI18nResource<string, string>(
            "Main",
            Cultures,
            Cultures.First()
        );
        foreach (var (c, i) in resource.Cultures.WithIndex())
        {
            Assert.IsTrue(resource.SetData(i.ToString(), GetValue(i.ToString(), c), c));
            for (var j = 0; j < resource.Cultures.Count; j++)
            {
                var key = j.ToString();
                if (j == i)
                {
                    // 同Key的值不是Default, 所以失败
                    Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                    Assert.IsFalse(
                        resource.SetDataWhenDefault(
                            key,
                            GetValue(key, c) + "_SetDataWhenDefault",
                            c
                        )
                    );
                    Assert.AreEqual(resource.GetData(key, c), GetValue(key, c));
                }
                else if (j < i)
                {
                    // 小于i代表此key被添加过, 但是默认值, 所以添加成功
                    Assert.AreEqual(resource.GetData(key, c), resource.GetDefaultValue("", null!));
                    Assert.IsTrue(
                        resource.SetDataWhenDefault(
                            key,
                            GetValue(key, c) + "_SetDataWhenDefault",
                            c
                        )
                    );
                    Assert.AreEqual(
                        resource.GetData(key, c),
                        GetValue(key, c) + "_SetDataWhenDefault"
                    );
                }
            }
        }
    }

    [TestMethod]
    public void RemoveData()
    {
        var resource = GetI18nResource();
        var dic = resource.DatasByKey.ToDictionary();
        var count = dic.Count;
        for (var i = 0; i < count; i++)
        {
            var pair = dic.Last();
            Assert.IsTrue(resource.RemoveData(pair.Key));
            dic.Remove(pair.Key);
            Assert.IsTrue(resource.DatasByKey.SequenceEqual(dic));
        }
        Assert.IsEmpty(resource.DatasByKey);
        Assert.IsTrue(resource.DatasByKey.SequenceEqual(dic));
    }
}
