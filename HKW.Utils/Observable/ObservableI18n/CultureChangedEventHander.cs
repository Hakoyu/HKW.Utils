using System.Globalization;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 文化改变事件参数
/// </summary>
/// <param name="sender">发送者</param>
/// <param name="e">参数</param>
public delegate void CultureChangedEventHander(
    ObservableI18nCore sender,
    CultureChangedEventArgs e
);
