---
title: "将删除的功能"
date: 2020-02-01T18:25:23+08:00
draft: false
weight: 20
---

##### 预计 2026-06-01 删除以下功能
Settings service 从 Properties 加载配置功能  

NeoLuna插件内的下列函数：  
std.Misc:EncodeToShareLinkMetaData(config) // 替换为 std.Misc:GetMetaData(config)  

##### 预计 2026-01-01 删除以下功能
NeoLuna插件内的下列函数：  
std.Sys:LusServSetIndex() // 改名为LuaServSetIndex()  

##### 2024-05-25 已删除以下功能
NeoLuna插件内的下列函数：  
std.Misc:AddV2cfgPrefix(b64Str)  
std.Misc:AddVmessPrefix(b64Str)  
std.Web:ExtractV2cfgLinks(text)  
std.Web:ExtractVmessLinks(text)  
std.Web:ExtractSsLinks(text)  