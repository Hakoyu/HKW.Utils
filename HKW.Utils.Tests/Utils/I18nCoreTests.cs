using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class I18nCoreTests
{
    public static CultureInfo[] Cultures { get; } =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures);
    public static CultureInfo CurrentCulture { get; } = CultureInfo.CurrentCulture;

    [TestMethod]
    public void ChangeSubResourceCulture()
    {
        var core = new I18nCore();
        core.CurrentCulture = Cultures[0];
        for (var i = 0; i < 10; i++)
        {
            core.AddResource(
                new ObservableI18nResource<string, string>(i.ToString(), Cultures, Cultures[0])
            );
        }
        for (var i = 0; i < Cultures.Length; i++)
        {
            var culture = Cultures[i];
            core.CurrentCulture = culture;
            Assert.AreEqual(culture, core.Resources.Values.Select(x => x.CurrentCulture));
        }
    }

    //[TestMethod]
    //public void ChangeCultureChangeTypes()
    //{
    //    var core = new I18nCore();
    //    Assert.AreEqual(
    //        core.CurrentCulture,
    //        [CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture]
    //    );

    //    var oldCulture = core.CurrentCulture;
    //    core.CultureChangeTypes = CultureChangeTypes.CultureInfoCurrentCulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(core.CurrentCulture, CultureInfo.CurrentCulture);
    //        Assert.AreEqual(oldCulture, CultureInfo.CurrentUICulture);
    //    }

    //    oldCulture = core.CurrentCulture;
    //    core.CultureChangeTypes = CultureChangeTypes.CultureInfoCurrentUICulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(core.CurrentCulture, CultureInfo.CurrentUICulture);
    //        Assert.AreEqual(oldCulture, CultureInfo.CurrentCulture);
    //    }

    //    core.CultureChangeTypes =
    //        CultureChangeTypes.CultureInfoCurrentCulture
    //        | CultureChangeTypes.CultureInfoCurrentUICulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(
    //            core.CurrentCulture,
    //            [CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture]
    //        );
    //    }

    //    core.CurrentCulture = CurrentCulture;
    //}
}
