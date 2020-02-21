local u={}

function AllServs()
    return Each(Server:GetAllServers())
end

function ToNumber(str)
    if string.isempty(str) then
        return 0
    end
    return tonumber(str)
end

function IsInTable(haystack, needle)
    assert(type(needle) == "string")
    assert(type(haystack) == "table")
	for _, item in pairs(haystack)
    do
        if string.lower(needle) == string.lower(item) then
            return true
        end
    end
    return false
end

local TimeZone8 = 8 * 60 * 60
local SecPerHour = 60 * 60
local SecPerDay = 24 * SecPerHour

function Ticks2Minutes(ticks)
    local r = ToNumber(ticks) / 60
    return math.floor(r)
end

function Ticks2Hours(ticks)
    local r = ToNumber(ticks) / SecPerHour
    return math.floor(r)
end

function Ticks2Days(ticks)
    local ticks8 = ToNumber(ticks) + TimeZone8
    local days = ticks8 / SecPerDay
    return math.floor(days)
end

u.TimeZone8 = TimeZone8
u.SecPerHour = SecPerHour
u.SecPerDay = SecPerDay
u.Timeout = Misc:GetTimeoutValue()

function u.ToNumber(str)
    return ToNumber(str)
end

function u.SecondDiff(tLeft, tRight)
    return math.abs(tLeft - tRight)
end

function u.MinuteDiff(tLeft, tRight)
    local mLeft = Ticks2Minutes(tLeft)
    local mRight = Ticks2Minutes(tRight)
    return math.abs(mLeft - mRight)
end

function u.HourDiff(tLeft, tRight)
    local hLeft = Ticks2Hours(tLeft)
    local hRight = Ticks2Hours(tRight)
    return math.abs(hLeft - hRight)
end

function u.DayDiff(tLeft, tRight)
    local dLeft = Ticks2Days(tLeft)
    local dRight = Ticks2Days(tRight)
    return math.abs(dLeft - dRight)
end

function u.Ticks2Days(ticks)
    return Ticks2Days(ticks)
end

function u.Ticks2Hours(ticks)
    return Ticks2Hours(ticks)
end

function u.GetUidOfServer(coreServ)
    if coreServ ~= nil then
        local coreState = coreServ:GetCoreStates()
        return coreState:GetUid()
    end
    return ""
end

function GetServsByMarks(marks, isContainsMark)
    assert(type(marks) == "table")
    assert(type(isContainsMark), "boolean")
    local hasMark = {}
    local noMark = {}
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        if IsInTable(marks, coreState:GetMark()) then
            table.insert(hasMark, coreServ)
        else
            table.insert(noMark, coreServ)
        end
    end
    if isContainsMark then
        return hasMark
    else
        return noMark
    end
end

function u.GetServersWithMarks(marks)
    return GetServsByMarks(marks, true)
end

function u.GetServersWithoutMarks(marks)
    return GetServsByMarks(marks, false)
end

function u.GetFirstServerWithMarks(marks)
    local servs = GetServsByMarks(marks, true)
    if #servs > 0 then
        return servs[1]
    end
    return nil
end

function u.GetFirstServerWithoutMarks(marks)
    local servs = GetServsByMarks(marks, false)
    if #servs > 0 then
        return servs[1]
    end
    return nil
end

function u.GetFirstServerWithName(name)
    assert(type(name) == "string")
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        if coreState:GetName() == name then
            return coreServ
        end
    end
    return nil
end

function u.GetFirstServerWithUid(uid)
    assert(type(uid) == "string")
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        if coreState:GetUid() == uid then
            return coreServ
        end
    end
    return nil
end

function u.GetServerByIndex(index)
    assert(type(index) == "number")
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        if coreState:GetIndex() == index then
            return coreServ
        end
    end
    return nil
end

function u.InvertServersSelection(servers)
    assert(type(servers) == "table")
    for k, coreServ in pairs(servers) do
        local coreState = coreServ:GetCoreStates()
        local selected = coreState:IsSelected()
        coreState:SetIsSelected(not selected)
    end
end

function u.UnSelectServers(servers)
    assert(type(servers) == "table")
    for k, coreServ in pairs(servers) do
        if coreServ ~= nil then
            local coreState = coreServ:GetCoreStates()
            coreState:SetIsSelected(false)
        end
    end
end

function u.SelectServers(servers)
    assert(type(servers) == "table")
    for k, coreServ in pairs(servers) do
        if coreServ ~= nil then
            local coreState = coreServ:GetCoreStates()
            coreState:SetIsSelected(true)
        end
    end
end

function u.SelectAll()
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        coreState:SetIsSelected(true)
    end
end

function u.InvertSelection()
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        local selected = coreState:IsSelected()
        coreState:SetIsSelected(not selected)
    end
end

function u.SelectNone()
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        coreState:SetIsSelected(false)
    end
end

function u.WriteLine(filename, content)
    assert(type(filename) == "string")
    assert(type(content) == "string")
	local file=io.open(filename, "a")
    file:write(content .. "\n")
    file:close()
end

function u.IsInTable(haystack, needle)
    return IsInTable(haystack, needle)
end

function u.IsInTablePartially(haystack, needle)
    assert(type(needle) == "string")
    assert(type(haystack) == "table")
	for _, item in pairs(haystack)
    do
        if string.find(string.lower(item), string.lower(needle)) then
            return true
        end
    end
    return false
end

return u