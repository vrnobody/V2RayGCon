-- 每月1号清除统计数据
local clearStatDayOfMonth = 1

local Utils = require('lua.libs.utils')

local lastClearStatTimestampKey = "ClearStatDataTimestamp"

local function ResetAllStat()
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local title = coreState:GetTitle()
        print("重置：", title)
        coreState:SetDownlinkTotal(0)
        coreState:SetUplinkTotal(0)
    end
end

local function ClearStatOn(dayOfMonth)
    local k = lastClearStatTimestampKey
    local m = os.date("%Y-%m")
    local s = Misc:ReadLocalStorage(k)
    if s == m then
        print("本月已重置流量统计信息")
        return
    end
    
    local d = os.date("%d")
    if Utils.ToNumber(d) < dayOfMonth then
        print("没到流量统计信息重置日")
        return
    end
    
    print("正在重置流量统计信息")
    ResetAllStat()
    Misc:WriteLocalStorage(k, m)
    print("完成")
end

ClearStatOn(clearStatDayOfMonth)

