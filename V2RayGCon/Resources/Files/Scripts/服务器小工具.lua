--[[
    这个脚本集合了选中重复服务器，打包服务器等几个小功能。
]]

-- 选中服务器的最大延迟
local maxTimeout = 5000

-- 打包服务器的标记
local packageMark = "服务器包"

local Utils = require('lua.libs.utils')
local Set = require('lua.modules.set')

local function ScanScreenQrcode()
    local text = Misc:ScanQrcode()
    if string.isempty(text) then
        Misc:Alert("没扫到二维码")
    else
        Misc:Input("解码结果：", text, 10)
    end
end

local function ShowSelectedServNum(n)
    Misc:Alert("共选中 " .. tostring(n) .. " 个服务器")
end

local function UpdateSubsNow()
    local proxyPort = Web:GetProxyPort()
    Web:UpdateSubscriptions(proxyPort)
    Server:UpdateAllSummary()
end

local function SelectDupServers()
    local cache = Set.new({})
    local servs = Server:GetAllServers()
    local count = 0
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local mark = coreState:GetMark()
        if mark == packageMark then 
            coreState:SetIsSelected(false)
        else
            local summary = coreState:GetSummary()
            local selected =  cache:Contains(summary)
            cache:Add(summary)
            if selected then
                count = count + 1
            end
            coreState:SetIsSelected(selected)
        end
    end
    ShowSelectedServNum(count)
end

local function AddOneServerToList(rows, coreState)
    local row = {
        coreState:IsSelected() and "✔" or "✖",
        coreState:GetIndex(),
        coreState:GetName(),
        coreState:GetSummary(),
        coreState:GetMark(),
        coreState:GetRemark(),
        coreState:GetDownlinkTotalInBytes(),
        coreState:GetUplinkTotalInBytes(),
    }
    table.insert(rows, row)
end

local function ListAllServers()
    local rows = {}
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        AddOneServerToList(rows, coreState)
    end
    local columns = {"选中", "序号", "名称", "摘要", "标记", "备注", "下行(Bytes)", "上行(Bytes)"}
    Misc:ShowData("服务器列表：", columns, rows, 3)
end

local function SelectFastServers()
    local cache = Set.new({})
    local servs = Server:GetAllServers()
    local count = 0
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local timeout = coreState:GetSpeedTestResult()
        local mark = coreState:GetMark()
        if mark ~= packageMark and timeout > 0 and timeout < maxTimeout then
            local summary = coreState:GetSummary()
            local selected = not cache:Contains(summary)
            cache:Add(summary)
            if selected then
                count = count + 1
            end
            coreState:SetIsSelected(selected)
        else
            coreState:SetIsSelected(false)
        end
    end
    ShowSelectedServNum(count)
end

local function CountSelectedServerNum()
    local servs = Server:GetAllServers()
    local count = 0
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        if coreState:IsSelected() then
            count = count + 1
        end
    end
    return count 
end

local function PackSelectdServers()
    local orgUid = nil
    local coreServ = Utils.GetFirstServerWithMarks({packageMark})
    local orgUid = Utils.GetUidOfServer(coreServ)
    local count = CountSelectedServerNum()
    local name = "🎁" .. tostring(count) .. "-" .. os.date('%m%d.%H%M')
    local uid = Server:PackSelectedServers(orgUid, name)
    coreServ = Utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreState = coreServ:GetCoreStates()
        coreState:SetIndex(0)
        coreState:SetMark(packageMark)
        Misc:RefreshFormMain()
    end
end

local function Main()
    
    local menu = {
        {"列出所有服务器信息", ListAllServers},
        {"选中摘要重复的服务器", SelectDupServers},
        {"选中快速服务(<" .. tostring( maxTimeout ) .. "ms)", SelectFastServers},
        {"将选中的服务器打包为[" .. packageMark .. "]", PackSelectdServers},
        {"扫描屏幕任意二维码", ScanScreenQrcode},
        {"更新订阅", UpdateSubsNow},
    }
    
    local choices = {}
    for i, _ in ipairs(menu) do
        table.insert(choices, menu[i][1])
    end
    
    repeat
        local idx = Misc:Choice("请选择(点取消退出)：", choices, true)
        local exit = idx < 1 or idx > table.length(choices) 
        if not exit then
            menu[idx][2]()
        end
    until exit
end

Main()