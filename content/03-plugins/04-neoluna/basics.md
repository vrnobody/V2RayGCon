---
title: "开始使用"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---

##### 简单脚本演示
{{< figure src="../../../images/neoluna/show_v.png" >}}

##### 使用Signal响应停止按钮
{{< figure src="../../../images/neoluna/signal_demo.gif" >}}

##### 修改服务器标记
{{< figure src="../../../images/neoluna/set_mark_demo.gif" >}}
脚本源码如下：
```lua
local wserv = std.Server:GetWrappedServerByIndex(1)
local c = 0
repeat
    c = c + 1
    wserv:SetMark(c)
    std.Misc:Sleep(1000)
until std.Signal:Stop()
```