-- 如果4000端口已在使用，可修改为其他端口
local url = "http://localhost:4000/"
local public = "./lua/webui"

-- confings
local version = "0.0.1.0"
local pageSize = 50

-- code
local haServ = require('lua.modules.httpServ').new()
local json = require('lua.libs.json')
local utils = require('lua.libs.utils')

local function Clamp(v, min, max)
    if v < min then
        return min
    end
    if v > max then
        return max
    end
    return v
end

local function Response(ok, result)
    local r = {
        ["ok"] = ok,
        ["r"] = result,
    }
    return json.encode(r)
end

function GetPageNumByIndex(param)
    local index = #param > 0 and tonumber(param[1]) or 1
    local pageNum = math.ceil(index / pageSize)
    return pageNum
end

function GetSerializedServers(ps)
    local pageNum = #ps > 0 and tonumber(ps[1]) or 1
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
        t["name"] = coreState:GetName()
        t["summary"] = coreState:GetSummary()
        t["uid"] = coreState:GetUid()
        t["on"] = coreCtrl:IsCoreRunning()
        table.insert(d, t)
    end
    
    local pages = math.ceil(servs.Count / pageSize)
    if pages < 1 then
        pages = 1
    end
    
    return {
        ["pages"] = pages,
        ["data"] = d,
    }
end 

function RestartServ(ps)
    local uid = #ps > 0 and ps[1] or ""
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        Server:StopAllServers()
        coreCtrl:RestartCore()
        return true
    end
    return false
end

function StopServ(ps)
    local uid = #ps > 0 and ps[1] or ""
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        coreCtrl:StopCore()
        return true
    end
    return false
end

function GetServerVersion()
    return version
end

function GetSettings()
    return Misc:GetUserSettings()
end

local function Handler(req)
    local ok, j = pcall(json.decode, req)
    if not ok then
        print("parse request error: ", req)
        return Response(false, "parse request error")
    end
    
    local fn = j["fn"]
    local f = _G[fn]
    local p = j["ps"]
    if type(f) ~= "function" then
        return Response(false, "function not found")
    end
    if type(p) ~= "table" then
        return Response(false, "params is not a table")
    end
    
    local ok, r = pcall(f, p)
    if ok then
        return Response(true, r)
    end
    return Response(false, "call " .. fn .. " error: " .. r)
end

local function Main()
    haServ:Create(url, public, Handler, false)
    print("请打开网址: ", url)
    haServ:Run()
end

Main()