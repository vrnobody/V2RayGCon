--[[
通过热键来切换服务器：

ctrl + 1 切换至第一个服务器
ctrl + 2 切换至第二个服务器
ctrl + 3 切换至随机tls服务器
ctrl + 4 关闭所有服务器

上面的数字键是QWER对上那行。
]]

local Utils = require('lua.libs.utils')
local hkMgr = require('lua.modules.hotkey').new()

local function Rnd(max)
    math.randomseed(os.time())
    return math.random(max)
end

local function SwitchToCoreServ(coreServ)
    if coreServ == nil then
        local err = "没有可用服务器!切换失败!"
        print(err)
        Misc:Alert(err)
        return
    end
    local coreState = coreServ:GetCoreStates()
    local latency = coreState:GetSpeedTestResult()
    local title = coreState:GetTitle()
    local coreCtrl = coreServ:GetCoreCtrl()
    print("切换至: " .. title)
    Server:StopAllServers()
    coreCtrl:RestartCore()
end

local function SwitchToServerByIndex(idx)
    local servs = Server:GetAllServers()
    local count = servs.Count
    if idx >= count then
        SwitchToCoreServ(nil)
    else
        SwitchToCoreServ(servs[idx])
    end
end

local function SwitchToRndTlsServ()
    local allServs = Server:GetAllServers()
    local coreServs = {}
    for coreServ in Each(allServs) do
        local coreState = coreServ:GetCoreStates()
        local summary = coreState:GetSummary()
        if string.find(summary, ".tls@") ~= nil then
            table.insert(coreServs, coreServ)
        end
    end
    local len = table.length(coreServs)
    if len < 1 then
        SwitchToCoreServ(nil)
        return
    end
    
    local idx = Rnd(len)
    SwitchToCoreServ(coreServs[idx])
end

local function Main()
    local hkCfgs = {
        {"D1", function() SwitchToServerByIndex(0) end},
        {"D2", function() SwitchToServerByIndex(1) end},
        {"D3", SwitchToRndTlsServ},
        {"D4", function() Server:StopAllServers() end},
    }
    
    for idx, hkCfg in ipairs(hkCfgs) do
        if not hkMgr:Reg(hkCfg[1], hkCfg[2], false, true, false) then
            local err = "注册热键[" .. hkCfg[1] .. "]失败"
            Misc:Alert(err)
            assert(false, err)
        end
    end
    
    while not Signal:Stop() do
        if hkMgr:Wait(5000) then
            Utils.GC()
        end
    end
    
    hkMgr:Destroy()
end

Main()