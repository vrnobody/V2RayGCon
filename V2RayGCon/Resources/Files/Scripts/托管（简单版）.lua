--[[
定时测速并打包服务器
--]]


-- 每20秒钟挑一个服务器出来测速
local speedtestTimespan = 20

-- 每测100个服务器后打包一次
local roundsPerPack = 100

-- 打包延迟少于多少毫秒的服务器
local maxLatency = 3000

-- 服务器包备注
local pkgRemark = "托管"

local Utils = require('lua.libs.utils')

local curIdx = -1

function PickNext()
    local servs = Server:GetAllServers()
    
    if servs.Count < 1 then
        return nil
    end
    
    local coreServ = PickNewServ(servs)
    if coreServ ~= nil then
        return coreServ
    end
    
    for i = 1, 100 do
        curIdx = (curIdx + 1) % servs.Count
        coreServ = servs[curIdx]
        local coreState = coreServ:GetCoreStates()
        if string.isempty(coreState:GetRemark()) then
            return coreServ
        end
    end
    return 
end

function PickNewServ(servs)
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local latency = coreState:GetSpeedTestResult()
        local remark = coreState:GetRemark()
        if latency < 1 and string.isempty(remark) then
            return coreServ
        end
    end
    return nil
end

function PackFastServers()
    local servs = Server:GetAllServers()
    local oldUid = ""
    local c = 0
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local remark = coreState:GetRemark()
        local latency = coreState:GetSpeedTestResult()
        
        if remark == pkgRemark then
            oldUid = coreState:GetUid()
            -- print("oldUid: ", oldUid)
        end
        
        if string.isempty(remark) and latency < maxLatency then
            coreState:SetIsSelected(true)
            c = c + 1
        else
            coreState:SetIsSelected(false)
        end
    end
    
    if c < 1 then
        return
    end
    
    local name = pkgRemark .. tostring(c)
    local uid = Server:PackSelectedServers(oldUid, name)
    if uid ~= oldUid then
        MarkNewPackage(uid)
    end
end

function MarkNewPackage(uid)
    local coreServ = Utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreState = coreServ:GetCoreStates()
        coreState:SetRemark(pkgRemark)
        coreState:SetIndex(0.5)
        Misc:RefreshFormMain()
    end
end

function Main()
    local counter = 0
    while not Signal:Stop() do
        
        Misc:Sleep( speedtestTimespan * 1000 )
        
        if not Signal:ScreenLocked() then
            local coreServ = PickNext()
            if coreServ ~= nil then
                local coreState = coreServ:GetCoreStates()
                local coreCtrl = coreServ:GetCoreCtrl()
                print("测试: ", coreState:GetTitle())
                coreCtrl:RunSpeedTest()
            end
            
            counter = counter + 1
            if counter >= roundsPerPack then
                print("打包")
                counter = 0
                PackFastServers()
            end
        end
    end
end

Main()