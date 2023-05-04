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

local function GetterCoreServInfo(coreServ)
    local t = {}
    if coreServ == nil then
        return t
    end
    local coreState = coreServ:GetCoreStates()
    local coreCtrl = coreServ:GetCoreCtrl()
    t["index"] = coreState:GetIndex()
    t["name"] = coreState:GetName()
    t["summary"] = coreState:GetSummary()
    t["uid"] = coreState:GetUid()
    t["on"] = coreCtrl:IsCoreRunning()
    return t
end

local function SearchAllServer(servs, first, last)
    local d = {}
    for i = first, last do
        local coreServ = servs[i]
        local t = GetterCoreServInfo(coreServ)
        table.insert(d, t)
    end
    return d
end

local function FilterServsByName(servs, keyword)
    local r = {}
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local name = coreState:GetName()        
        if string.find(name, keyword) then
            table.insert(r, coreServ)
        end
    end
    return r
end

local function FilterServsBySummary(servs, keyword)
    local r = {}
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local summary = coreState:GetSummary()        
        if string.find(summary, keyword) then
            table.insert(r, coreServ)
        end
    end
    return r
end

local function FilterServs(servs, searchType, keyword)
    if searchType == "title" then
        return FilterServsByName(servs, keyword)
    end
    return FilterServsBySummary(servs, keyword)
end

local function CalcTotalPageNumber(total)
    local pages = math.ceil(total / pageSize)
    if pages < 1 then
        pages = 1
    end
    return pages
end

function GetSerializedServers(ps)
    utils.ToNumber(str)
    local pageNum = #ps > 0 and tonumber(ps[1]) or 1
    local searchType = #ps > 1 and ps[2] or ""
    local keyword = #ps > 2 and ps[3] or ""
    -- print("params: ", pageNum, ", ", searchType, ", ", keyword)
    local servs = Server:GetAllServers()
    local r = {
        ["pages"] = 0,
        ["data"] = {},
    }
    
    if servs.Count < 1 then
        return r
    end
    
    local min = 0
    local max = servs.Count - 1
    local first = Clamp((pageNum - 1) * pageSize, min, max)
    local last = Clamp((pageNum) * pageSize - 1, first, max)
    if string.isempty(searchType) or string.isempty(keyword) then
        -- print("search all")
        r["pages"] = CalcTotalPageNumber(servs.Count)
        r["data"] = SearchAllServer(servs, first, last)
        return r
    end
    
    if searchType == "index" then
        local idx = math.floor((tonumber(keyword) or 0) - 1)
        if idx < 0 or idx >= servs.Count then
            return r
        end
        r["pages"] = 1
        r["data"] = { GetterCoreServInfo(servs[idx]) }
        return r
    end
    
    local filtered = FilterServs(servs, searchType, keyword)
    local max = #filtered
    r["pages"] = CalcTotalPageNumber(#filtered)
    local d = {}
    for i = first + 1, last + 1 do
        if i <= max then
            local coreServ = filtered[i]
            local t = GetterCoreServInfo(coreServ)
            table.insert(d, t)
        end
    end
    r["data"] = d
    return r
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

function SaveServerConfig(ps)
    local uid = #ps > 0 and ps[1] or ""
    local config = #ps > 1 and ps[2] or ""
    
    if string.isempty(config) then
        return "config is empty!"
    end
    
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ == nil then
        return "server not found!"
    end
    
    local coreConfiger = coreServ:GetConfiger()
    coreConfiger:SetConfig(config)
    return nil
end

function GetServerConfig(ps)
    local uid = #ps > 0 and ps[1] or ""
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ then
        local coreConfiger = coreServ:GetConfiger()
        local config = coreConfiger:GetConfig()
        return config
    end
    return nil
end

function DeleteServersByUids(ps)
    local uids = #ps > 0 and ps[1] or {}
    if type(uids) ~= "table" then
        return false
    end
    Server:DeleteServerByUids(uids)
    return true
end

function ImportShareLinks(ps)
    local links = #ps > 0 and ps[1] or ""
    local mark = #ps > 1 and ps[2] or ""
    local c = Misc:ImportLinks(links, mark)
    Misc:RefreshFormMain()
    return c
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
    local ver = GetServerVersion()
    print("server.lua v", ver)
    haServ:Create(url, public, Handler, false)
    print("请打开网址: ", url)
    haServ:Run()
end

Main()