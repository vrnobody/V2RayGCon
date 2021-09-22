--[[
查询服务器所在国家然后保存到tag1中
2021-09-22
--]]

local geoAPI = "http://ip-api.com/json/"

local json = require('lua.libs.json')

local proxyPort = Web:GetProxyPort()
local servs = Server:GetAllServers()

function IsDupSummary(summary, index)
    for i = 0, index - 1 do
        local coreServ = servs[i]
        local coreState = coreServ:GetCoreStates()
        local s = coreState:GetSummary()
        if s == summary then
            return true
        end
    end
    return false
end

function GetHost(summary)
    local t = {}
    for str in string.gmatch(summary, "([^@]+)") do
        table.insert(t, str)
    end
    if table.length(t) == 2 then
        return t[2]
    end
    return nil
end

function GetGeoInfo(host)
    local url = geoAPI .. host
    local resp = Web:Fetch(url, proxyPort, 5000)
    Misc:Sleep(300)
    local o = json.decode(resp)
    return o["country"]
end

function TryGetGeoInfo(host)
    for i = 0, 10 do
        local ok, country = pcall(GetGeoInfo, host)
        if ok then
            return country
        end
        Misc:Sleep(5000)
        print("Error: ", country)
    end
    return nil
end

function Proc(coreServ, index)
    local coreState = coreServ:GetCoreStates()
    local summary = coreState:GetSummary()
    if not IsDupSummary(summary, index) then
        local host = GetHost(summary)
        if host ~= nil then
            local title = coreState:GetTitle()
            local country = TryGetGeoInfo(host)
            if country ~= nil then
                coreState:SetTag1(country)
                print(title, " -> ", country)
            else
                print("Get GEO info failed: ", title)
            end
        end
    end
end

function Main()
    local max = servs.Count
    for i = 0, max - 1 do
        local coreServ = servs[i]
        local coreState = coreServ:GetCoreStates()
        local tag1 = coreState:GetTag1()
        if string.isempty(tag1) then
            Proc(coreServ, i)
        end
        if Signal:Stop() then
            return
        end
    end
end

Main()