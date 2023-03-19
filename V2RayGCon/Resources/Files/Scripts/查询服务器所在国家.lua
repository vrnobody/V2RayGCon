--[[
查询服务器所在国家然后保存到tag1中
2023-03-19
--]]

local geoAPI = "http://ip-api.com/json/"

local utils = require('lua.libs.utils')
local json = require('lua.libs.json')

local proxyPort = Web:GetProxyPort()
local cache = {}

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
    return o["countryCode"]
end

function TryGetGeoInfo(host)
    local c = cache[host]
    if c ~= nil then
        print("using cache for ", host)
        return c
    end
    for i = 0, 5 do
        if Signal:Stop() then
            return nil
        end
        local ok, country = pcall(GetGeoInfo, host)
        if ok then
            cache[host] = country
            return country
        end
        Misc:Sleep(5000)
        print("error: ", country)
    end
    return nil
end

function Proc(coreServ)
    local coreState = coreServ:GetCoreStates()
    local tag1 = coreState:GetTag1()
    if not string.isempty(tag1) then
        return
    end
    local summary = coreState:GetSummary()
    local host = GetHost(summary)
    if host ~= nil then
        local title = coreState:GetTitle()
        local country = TryGetGeoInfo(host)
        if country ~= nil then
            coreState:SetTag1(country)
            print(title, " -> ", country)
        else
            print("fail to get GEO info: ", title)
        end
    end
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