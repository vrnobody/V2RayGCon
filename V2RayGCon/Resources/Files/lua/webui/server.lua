-- 如果4000端口已在使用，可修改为其他端口
local url = "http://localhost:4000/"
local public = "./lua/webui"

local pageSize = 50

local haServ = require('lua.modules.httpServ').new()
local json = require('lua.libs.json')
local utils = require('lua.libs.utils')

function Main()
    haServ:Create(url, public, handler, false)
    print("请打开网址: ", url)
    haServ:Run()
end

function Clamp(v, min, max)
    if v < min then
        return min
    end
    if v > max then
        return max
    end
    return v
end

function GetPageNumByIndex(index)
    local pageNum =math.ceil(index / pageSize)
    return tostring(pageNum)
end

function GetSerializedServers(pageNum)
    local servs = Server:GetAllServers()
    if servs.Count < 1 then
        return json.encode({
            ["pages"] = 0,
            ["data"] = {},
        })
    end
    
    local min = 0
    local max = servs.Count - 1
    local first = Clamp((pageNum - 1) * pageSize, min, max)
    local last = Clamp((pageNum) * pageSize - 1, first, max)
    
    -- print(first, '-', last)
    
    local d = {}
    for i = first, last do
        local coreServ = servs[i]
        local coreState = coreServ:GetCoreStates()
        local coreCtrl = coreServ:GetCoreCtrl()
        local t = {}
        t["index"] = coreState:GetIndex()
        t["name"] = coreState:GetShortName()
        t["summary"] = coreState:GetSummary()
        t["uid"] = coreState:GetUid()
        t["on"] = coreCtrl:IsCoreRunning()
        table.insert(d, t)
    end
    
    local pages = math.ceil(servs.Count / pageSize)
    if pages < 1 then
        pages = 1
    end
    
    local r = {
        ["pages"] = pages,
        ["data"] = d,
    }
    
    -- print("d: ", #d)
    return json.encode(r)
end 

function RestartServ(uid)
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        Server:StopAllServers()
        coreCtrl:RestartCore()
        return "ok"
    end
    return "unknow uid: " .. uid
end

function StopServ(uid)
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        coreCtrl:StopCore()
        return "ok"
    end
    return "unknow uid: " .. uid
end

function GetSettings()
    local s = Misc:GetUserSettings()
    return s
end

function handler(req)
    
    -- print("req: ", req)
    
    local j = json.decode(req)
    local op = j["fn"]
    
    -- print(table.dump(j))
    -- print("fn: ", op, "param: ", table.dump(j["ps"]))
    
    if op == "GetSettings" then
        return GetSettings()
    elseif op == "GetPageNumByIndex" then
        local index = #j["ps"] > 0 and tonumber(j["ps"][1]) or 1
        return GetPageNumByIndex(index)
    elseif op == "GetSerializedServers" then
        local pn = #j["ps"] > 0 and tonumber(j["ps"][1]) or 1
        return GetSerializedServers(pn)
    elseif op == "RestartServ" then
        return RestartServ(j["ps"][1])
    elseif op == "StopServ" then
        return StopServ(j["ps"][1])
    end
    return "unknow req: " .. req
end

Main()