# Copilot Instructions

## General Guidelines
- 所有注释必须使用简体中文输出, 若注释不为中文则翻译成简体中文。
- 用户关注性能细节，倾向减少循环内分支判断。
- 代码优化以性能优先。
- 异常信息应使用英文。
- 当布尔取反时不使用`!`, 而是使用`is false`。
- 用户要求修改代码时保留原有注释，不要删除或丢失注释内容。

### 注释中的代码示例使用remarks
使用 remarks + CDATA 来包含代码示例, 以避免在生成的文档中出现 HTML 转义字符。
示例:
```csharp
/// <remarks><![CDATA[
/// ...
/// ]]></remarks>
```

### 特定条件下优先使用LINQ
示例1:
```csharp
// 原代码:
forearch (var i in ints)
{
	if (i == 0)
		continue;
	Console.WriteLine(int);
}
// 优先使用: 
forearch (var i in ints.Where(x => x != 0))
{
	Console.WriteLine(int);
}
```
示例2
```csharp
// 原代码
forearch (var i in ints)
{
	if (i == 0)
		return;
}
// 优先使用: 
if (ints.Any(i => i == 0))
	return;
```

