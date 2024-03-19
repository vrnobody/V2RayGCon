---
title: "全部函数"
date: 2020-02-02T19:13:26+08:00
draft: false
weight: 1000
---

前面的示例脚本中像 `std.Signal:Stop()` 这样的语句，实际是调用了 [ILuaSignal.cs][1] 里面的 `bool Stop();` 在 [Interfaces][2] 目录下的所有接口都可以像上面那样调用。例如：`std.Misc:Sleep(1000)`

控制服务器的脚本，通常从调用 `std.Server:GetAllServers()` 函数开始。  
下面是一个选中所有 ws.tls 服务器的小脚本：  
```lua
local servs = std.Server:GetAllServers()
foreach coreServ in servs do
    local wserv = coreServ:Wrap()
    local summary = wserv:GetSummary()
    local isWsTls = string.startswith(summary, "vless.ws.tls@")
    wserv:SetIsSelected(isWsTls == true)
end
```
其中：  
 * foreach 是 NeoLuna 特有函数，用于遍历 CSharp 的集合
 * string.startswith() 是预定义函数，源码在 [LuaPredefinedFunctions.txt][4]
 * coreServ:Wrap() 把 [ICoreServCtrl][3] 包装成 [IWrappedCoreServCtrl][5]，使用起来更方便

上面代码使用 `coreServ`，`wserv` 这么奇怪的变量名是因为这两个关键字有代码提示。  
  
[1]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/NeoLua/Interfaces/ILuaSignal.cs "ILuaSignal.cs"
[2]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/NeoLua/Interfaces "Interfaces"
[3]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/ICoreServCtrl.cs "ICoreServCtrl.cs"
[4]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/NeoLua/Resources/Files/LuaPredefinedFunctions.txt "LuaPredefinedFunctions.txt"
[5]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/IWrappedCoreServCtrl.cs "IWrappedCoreServCtrl"