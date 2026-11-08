using System.Globalization;

namespace HKW.HKWUtils;

// 仅库内资源参加分阶段文化提交, 不把任意外部 setter 当作可回滚事务.
internal interface II18nCultureParticipant
{
    II18nCultureChange CaptureCultureChange(CultureInfo? culture);
}

internal interface II18nCultureChange : IDisposable
{
    void Commit();
    void Publish();
}
