--[[
查询出口所在国家然后保存到tag1中
e.g.  CN=>US
2023-06-03
--]]

local geoAPI = "http://ip-api.com/json/"

local utils = require('lua.libs.utils')
local json = require('lua.libs.json')

local cache = {}

function GetHostFromSummary(summary)
    local t = {}
    for str in string.gmatch(summary, "([^@]+)") do
        table.insert(t, str)
    end
    if table.length(t) == 2 then
        return t[2]
    end
    return nil
end

function GetGeoInfo(coreServ)
    local coreState = coreServ:GetCoreStates()
    
    local title = coreState:GetTitle()
    print(title)
    
    local summary = coreState:GetSummary()
    local tag = cache[summary]
    if not string.isempty(tag) then
        print("using cache tag ", tag)
        coreState:SetTag1(tag)
        return
    end

    local cLocal = ""
    local cRemote = ""
    local ok, result = pcall(GetGeoInfoCore, coreServ, true)
    if ok and not string.isempty(result) then
        cLocal = result
    else
        print("get local geo info failed. ", result)
    end
    ok, result = pcall(GetGeoInfoCore, coreServ, false)
    if ok and not string.isempty(result) then
        cRemote = result
    else
        print("get remote geo info failed. ", result)
    end
    
    if not string.isempty(cLocal) or not string.isempty(cRemote) then
        tag = cLocal .. "=>" .. cRemote
        coreState:SetTag1(tag)
        cache[summary] = tag
        print(tag)
    else
        print("get tag failed")
    end
end

function GetGeoInfoCore(coreServ, islocal)
    local url = geoAPI
    if islocal then
        local coreState = coreServ:GetCoreStates()
        local summary = coreState:GetSummary()
        local host = GetHostFromSummary(summary)
        url = url .. host
    end
    local coreCtrl = coreServ:GetCoreCtrl()
    for i = 1, 3 do
        local resp = coreCtrl:Fetch(url, 4000)
        if not string.isempty(resp) then
            local o = json.decode(resp)
            return o["countryCode"]
        end
        if Signal:Stop() then
            return ""
        end
        -- print("retry")
        Misc:Sleep(2000)
    end
    return ""
end

function Proc(coreServ)
    local coreState = coreServ:GetCoreStates()
    
    local remark = coreState:GetRemark()
    if not string.isempty(remark) then
        return
    end
    
    local latency = coreState:GetSpeedTestResult()
    if latency == utils.Timeout then
        return
    end
    
    local tag1 = coreState:GetTag1()
    if not string.isempty(tag1) then
        local summary = coreState:GetSummary()
        cache[summary] = tag1
        return
    end
    
    GetGeoInfo(coreServ)    
end

function Main()
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        Proc(coreServ, i)
        if Signal:Stop() then
            return
        end
    end
end

Main()