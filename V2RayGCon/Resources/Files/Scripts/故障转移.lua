--[[
定时测试服务器是否可用，
出现故障时切换至随机服务器。
注意：Inbound必须是http协议。
--]]

-- Ping测试目标网址
local url = "https://www.google.com"

-- Ping间隔时长（毫秒）
local pingInterval = 5000

-- Ping重试次数
local maxRetry = 3

-- 只切换到测速延迟小于以下数值的服务器
local maxLatency = 5000

function Init()
    local now = os.time()
    math.randomseed(now)
end

function Ping()
    local proxyPort = Web:GetProxyPort()
    for i = 1, maxRetry do
        local ok = Web:Tcping(url, 5000, proxyPort)
        if ok then
            return ok
        end
        print("Retry ...")
    end
    return false
end

function SwitchToRandomServer()
    local servs = Server:GetAllServers()
    local n = 0
    local r = math.random(servs.Count)
    for coreServ in Each(servs) do
        n = n + 1
        local coreState = coreServ:GetCoreStates()
        local latency = coreState:GetSpeedTestResult()
        if n >= r and maxLatency > latency then
            local title = coreState:GetTitle()
            print("Switch to: ", title)
            Server:StopAllServers()
            local coreCtrl = coreServ:GetCoreCtrl()
            coreCtrl:RestartCore()
            return
        end
    end
end
        

function Main()
    Init()
    repeat
        Misc:Sleep(pingInterval)
        local ok = Ping()
        if not ok then
            print("Ping failed.")
            SwitchToRandomServer()
        else
            print("Ping OK.")
        end
    until Signal:Stop()
end

Main()