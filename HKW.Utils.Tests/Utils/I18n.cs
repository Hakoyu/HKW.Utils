//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using HKW.HKWUtils;

//namespace HKW.HKWUtilsTests.Utils;

//[TestClass]
//public class I18n
//{
//    [TestMethod]
//    public void I18nCoreOnCurrentCultureChanged()
//    {
//        Thread.CurrentThread.CurrentCulture =
//            Thread.CurrentThread.CurrentUICulture =
//            CultureInfo.CurrentUICulture =
//                CultureInfo.CurrentCulture;
//        var core = new I18nCore();
//        var baseCulture = core.CurrentCulture;
//        var targetCulture = CultureInfo.GetCultureInfo("en");
//        Assert.IsTrue(core.ChangeThreadCulture is false);
//        Assert.IsTrue(core.ChangeThreadUICulture is false);

//        Assert.AreEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.CurrentCulture = targetCulture;
//        Assert.AreNotEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreNotEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreNotEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreNotEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.ChangeThreadCulture = true;
//        core.CurrentCulture = targetCulture;
//        Assert.AreEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreNotEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreNotEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.CurrentCulture = baseCulture;
//        Assert.AreEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.ChangeThreadCulture = false;
//        core.ChangeThreadUICulture = true;
//        core.CurrentCulture = targetCulture;
//        Assert.AreNotEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreNotEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.CurrentCulture = baseCulture;
//        Assert.AreEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);

//        core.ChangeThreadCulture = true;
//        core.ChangeThreadUICulture = true;
//        core.CurrentCulture = baseCulture;
//        Assert.AreEqual(CultureInfo.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(CultureInfo.CurrentUICulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentCulture, core.CurrentCulture);
//        Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, core.CurrentCulture);
//    }
//}
