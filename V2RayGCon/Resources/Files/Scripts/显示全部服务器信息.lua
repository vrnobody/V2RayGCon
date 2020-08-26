local function AddOneRow(rows, coreState)
    local row = {
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

local function Main()
    local rows = {}
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        AddOneRow(rows, coreState)
    end
    local columns = {"序号", "名称", "摘要", "标记", "备注", "下行", "上行"}
    Misc:ShowData("服务器列表：", columns, rows, 2)
end

Main()