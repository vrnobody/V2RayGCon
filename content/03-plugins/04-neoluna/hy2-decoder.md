---
title: "导入 hy2://... 链接"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 50
---

hysteria2 支持把分享链接写进配置，所以手搓个解码器很简单。  

##### 添加hy2自定义core
{{< figure src="../../../images/neoluna/hy2_custom_core_settings.png" >}}  
注：需要下载 hysteria2 内核放到 3rd/hy2 目录内  

##### 添加hy2http配置模板
```yaml
http:
  listen: %host%:%port%
```

##### 执行以下代码
```lua
-- 自定义 core 名称
local customCoreName = "hy2"

-- 自定义 inbound 模板
local customInboundTemplateName = "hy2http"

-- 设置标记
local customMark = "hy2serv"

-- 代码
local hasUriDecodeFunc = std.Sys:GetAppVersion() >= "1.8.9.2"

local function ExtractHy2ShareLinks(s)
    local r = {}
    local lines = string.split(s, '\r\n" ')
    for _, line in ipairs(lines) do
        if string.startswith(line, "hy2://")
            or string.startswith(line, "hysteria2://")
        then
            table.insert(r, line)
        end
    end
    return r
end

local function GetName(link)
    if not hasUriDecodeFunc then
        return ""
    end
    local ps = string.split(link, "#")
    return #ps > 1 and std.Web:UriDecode(ps[#ps]) or ""
end

local function GenConfig(link)
    local http = [[
http:
  listen: 127.0.0.1:8080

]]
    return http .. "server: " .. link
end

local function ChangeServSettings(uid)
    local wserv = std.Server:GetWrappedServerByUid(uid)
    if wserv == nil then
        return
    end
    wserv:SetCustomCoreName(customCoreName)
    wserv:SetInboundName(customInboundTemplateName)
end

local function Import(links)
    local c = 0
    for _, link in ipairs(links) do
        local name = GetName(link)
        local config = GenConfig(link)
        local uid = std.Server:AddNew(name, config, customMark)
        if not string.isempty(uid) then
            c = c + 1
            ChangeServSettings(uid)
            print("成功：", link)
        else
            print("失败：", link)
        end
    end
    return "导入了[" .. c .. "]个服务器"
end

local function Main()
    local text = std.Misc:Input("hy2://...链接：", 10)
    local msg = "内容为空"
    if not string.isempty(text) then
        local links = ExtractHy2ShareLinks(text)
        msg = Import(links)
    end
    std.Misc:Alert(msg)
end

Main()
```
