using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Tests.Utils;

[TestClass]
public class I18n
{
    [TestMethod]
    public void I18nCoreOnCurrentCultureChanged()
    {
        Thread.CurrentThread.CurrentCulture =
            Thread.CurrentThread.CurrentUICulture =
            CultureInfo.CurrentUICulture =
                CultureInfo.CurrentCulture;
        var core = new I18nCore();
        var baseCulture = core.CurrentCulture;
        var targetCulture = CultureInfo.GetCultureInfo("en");
        Assert.IsTrue(core.ChangeThreadCulture is false);
        Assert.IsTrue(core.ChangeThreadUICulture is false);

        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentUICulture);

        core.CurrentCulture = targetCulture;
        Assert.IsTrue(core.CurrentCulture != CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture != CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture != Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture != Thread.CurrentThread.CurrentUICulture);

        core.ChangeThreadCulture = true;
        core.CurrentCulture = targetCulture;
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture != CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture != Thread.CurrentThread.CurrentUICulture);

        core.CurrentCulture = baseCulture;
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentUICulture);

        core.ChangeThreadCulture = false;
        core.ChangeThreadUICulture = true;
        core.CurrentCulture = targetCulture;
        Assert.IsTrue(core.CurrentCulture != CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture != Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentUICulture);

        core.CurrentCulture = baseCulture;
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentUICulture);

        core.ChangeThreadCulture = true;
        core.ChangeThreadUICulture = true;
        core.CurrentCulture = baseCulture;
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == CultureInfo.CurrentUICulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentCulture);
        Assert.IsTrue(core.CurrentCulture == Thread.CurrentThread.CurrentUICulture);
    }
}
