--[[
添加、删除、启动、停止服务器测试。
用这个脚本做压力测试时，应该开启多个脚本编辑器，同时运行多个实例。
2023-07-31
--]]

local minDelay = 150
local maxDelay = 300

local task = require('lua.modules.task').new()
local json = require('lua.libs.json')

local config = {}

function RandomWait(max)
    max = max and math.max(minDelay, max) or maxDelay
    local d = math.random(minDelay, max)
    task:Wait(d)
end    

function RandomPick(max)
    return math.random(1, max)
end

local serial = 0
function AddServ()
    while true do
        RandomWait()
        local count = Server:Count()
        if count < 50 then
            serial = serial + 1
            local name = "#" .. tostring(serial)
            config["v2raygcon"]["alias"] = name
            local cfg = json.encode(config)
            local r = Server:Add(cfg)
            print("add server", name, ":", r)
        end
    end
end

function RemoveServ()
    while true do
        RandomWait()
        local cserv = Server:GetServByIndex(1)
        if cserv ~= nil then
            local uid = cserv:GetUid()
            local title = cserv:GetTitle()
            Server:DeleteServerByUids({uid})
            print("remove server:", title)
        end
    end
end

function SetServIndex()
    while true do
        RandomWait()
        local count = Server:Count()
        local old = RandomPick(count)
        local cserv = Server:GetServByIndex(old)
        if cserv ~= nil then        
            local new = RandomPick(count)
            cserv:SetIndex(nex)
            print("set index from", old, "to", new)
        end
    end
end

function StartServ()
    while true do
        RandomWait()
        Server:StopAllServers()
        local count = Server:Count()
        local index = RandomPick(count)
        local cserv = Server:GetServByIndex(index)
        if cserv ~= nil then
            local port = math.random(10000, 60000)
            cserv:SetInboundAddr("127.0.0.1", port)
            cserv:RestartCore()
            local title = cserv:GetTitle()
            print("start server:", title)
        end
    end
end

function Watcher()
    repeat
        local count = Server:Count()
        task:Wait(1000)
    until Signal:Stop()
    task:Stop()
end

function Main()
    math.random(os.time())
    ParseConfig()
    
    task:Init(Watcher)
    
    task:Init(AddServ)
    task:Init(RemoveServ)
    task:Init(SetServIndex)
    task:Init(StartServ)
    
    task:Run()
end

function ParseConfig()
    local cfg = 
[[{
  "log": {
    "loglevel": "warning"
  },
  "v2raygcon": {
    "alias": "1",
    "description": ""
  },
  "inbounds": [
    {
      "tag": "agentin",
      "port": 1080,
      "listen": "127.0.0.1",
      "protocol": "socks",
      "settings": {}
    }
  ],
  "outbounds": [
    {
      "protocol": "vless",
      "settings": {
        "vnext": [
          {
            "address": "1.2.3.4",
            "port": 443,
            "users": [
              {
                "id": "dfa0db60-96bf-463a-9b15-adec36ea21ae",
                "encryption": "none"
              }
            ]
          }
        ]
      },
      "tag": "agentout",
      "streamSettings": {
        "network": "ws",
        "security": "tls",
        "wsSettings": {
          "path": "/"
        },
        "tlsSettings": {
          "serverName": "baidu.com"
        }
      }
    }
  ]
}
]]
    config = json.decode(cfg)
end

Main()