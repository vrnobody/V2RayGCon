---
title: "热切换节点"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 60
---

以前想给服务器包添加、删除节点，通常要用 Pacman 插件或者 lua 脚本重新打包生成配置，然后替换当前服务器的 config 并重启。这会导致所有连接断开，体验不是很好。xray/v2ray 支持通过 GRPC 调用 API 接口来热增删节点。但是它没提供查询当前节点的方法，删除节点永远返回成功 "{}"。于是调用 API 一段时间后，只有上帝和 gdb 知道 core 里面究竟还剩下哪些节点。  

为了改进 core 的 API 体验，于是有了 [xraye](https://github.com/vrnobody/xraye) 这个项目。这个项目 fork 自 [Xray-core](https://github.com/xtls/xray-core)，修改、添加了一些 API 命令，让热增删节点功能更易用。下面是一个通过调用 API 把测速结果小于 5000ms 的节点替换掉第一个服务器 outbounds 的示例。  

先下载修改版 xray 解压并替换掉原来的 xray.exe（也可以配置自定义内核，把修改版 xray.exe 放到其他文件夹）。  
  
然后配置一个开启了 http/socks/api 功能的模板，应用到第一个服务器上。示例中的 API 端口是 12345。  
```json
{
  "log": {
    "loglevel": "warning"
  },
  "inbounds": [
    {
      "tag": "http",
      "protocol": "http",
      "port": %port%,
      "listen": "%host%",
      "settings": {}
    },
    {
      "tag": "socks",
      "protocol": "socks",
      "port": 1080,
      "listen": "%host%",
      "settings": {
        "udp": true
      }
    },
    {
      "listen": "127.0.0.1",
      "port": 12345,
      "protocol": "dokodemo-door",
      "settings": {
        "address": "%host%"
      },
      "tag": "api-in"
    }
  ],
  "api": {
    "tag": "api-serv",
    "services": [
      "HandlerService"
    ]
  },
  "routing": {
    "rules": [
      {
        "inboundTag": [
          "api-in"
        ],
        "outboundTag": "api-serv",
        "type": "field"
      },
      {
        "type": "field",
        "inboundTag": [
          "http",
          "socks"
        ],
        "balancerTag": "pacman"
      }
    ],
    "balancers": [
      {
        "tag": "pacman",
        "selector": [
          "agentout"
        ]
      }
    ]
  }
}
```

接着启动服务器（不然没法用 API 命令）  

最后在 NeoLuna 插件中运行以下脚本：  
```lua
-- 修改版xray的位置
local xray = "./3rd/core/xray.exe"

-- 只打包测速结果小于5000毫秒的服务器
local latency = 5000

-- API端口
local apiPort = 12345

-- 把多个服务器打包成配置包
local function PackServers(latency)
    local uids = {}
    local servs = std.Server:GetAllServers()
    foreach coreServ in servs do
        local wserv = coreServ:Wrap()
        local index = wserv:GetIndex()
        if index ~= 1 then
            local r = wserv:GetSpeedTestResult() 
            if r < latency then
                table.insert(uids, wserv:GetUid())
            end
        end
    end
    return std.Server:PackServersToString(uids)
end

-- 辅助函数，执行API命令
local function ExecApiCmd(port, cmd, arg, stdin)
    local timeout = 10 * 1000
    local args = 'api ' .. cmd .. ' --server=127.0.0.1:' .. tostring(port)
    if not string.isempty(arg) then
        args = args .. " " .. arg
    end
    print(xray, args)
    local r = std.Sys:RunAndGetResult(xray, args, nil, stdin, timeout, nil, nil)
    -- for debugging
    print(r)
end

local function Main()

    -- 调用API清除所有outbounds
    ExecApiCmd(apiPort, "rmo", '"*"', nil)
    
    -- 生成配置包
    local config = PackServers(latency)
    
    -- 调用API把配置包添加到服务器中
    ExecApiCmd(apiPort, "ado", nil, config)
    
    -- 把配置包存入第一个服务器
    local wserv = std.Server:GetWrappedServerByIndex(1)
    wserv:SetConfigQuiet(config)
end

Main()
```
  
上面只是个简单的示例，还可以有更多玩法。  
