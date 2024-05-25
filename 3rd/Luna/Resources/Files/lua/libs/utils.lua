-- constants
local TimeZone8 = 8 * 60 * 60
local SecPerHour = 60 * 60
local SecPerDay = 24 * SecPerHour

-- export utils
local u={}

u.TimeZone8 = TimeZone8
u.SecPerHour = SecPerHour
u.SecPerDay = SecPerDay
u.Timeout = Misc:GetTimeoutValue()

-- helper functions 
local function ToNumber(s)
    return tonumber(s) or 0
end

local function ToLuaTicks(cSharpTicks)
    local t = ToNumber(cSharpTicks)
    return math.floor(t / 10000000 - 62135596800)
end

local function IsInTable(haystack, needle)
    if string.isempty(needle) or #haystack < 1 then
        return false
    end
    local tNeedle = type(needle)
    local tHaystack = type(haystack)
    assert(tNeedle == "string", "needle is " .. tNeedle)
    assert(tHaystack == "table", "haystack is " .. tHaystack)
	for _, item in pairs(haystack) do
        if string.lower(needle) == string.lower(item) then
            return true
        end
    end
    return false
end

local function Ticks2Minutes(ticks)
    local r = ToNumber(ticks) / 60
    return math.floor(r)
end

local function Ticks2Hours(ticks)
    local r = ToNumber(ticks) / SecPerHour
    return math.floor(r)
end

local function Ticks2Days(ticks)
    local ticks8 = ToNumber(ticks) + TimeZone8
    local days = ticks8 / SecPerDay
    return math.floor(days)
end

function u.ToLuaTable(ienum)
    local r = {}
    if ienum == nil or type(ienum) ~= "userdata" then
        return r
    end
    local et = ienum:GetEnumerator()
    while et:MoveNext() do
        table.insert(r, et.Current)
    end
    et:Dispose()
    return r
end

local function GetAllServs()
    local servs = {}
    local et = Server:GetAllServers():GetEnumerator()
    while et:MoveNext() do
        table.insert(servs, et.Current)
    end
    et:Dispose()
    return servs
end

local function GetAllWrappedServs()
    local servs = {}
    local et = Server:GetAllServers():GetEnumerator()
    while et:MoveNext() do
        table.insert(servs, et.Current:Wrap())
    end
    et:Dispose()
    return servs
end

local function GetServsByMarks(marks, isRemark, isContains)
    
    assert(type(marks) == "table")
    assert(type(isContains), "boolean")
    assert(type(isRemark), "boolean")
    
    local with = {}
    local without = {}
    for _, coreServ in ipairs(GetAllServs()) do
        local wserv = coreServ:Wrap()
        local mark = isRemark and wserv:GetRemark() or wserv:GetMark()
        if IsInTable(marks, mark) then
            table.insert(with, coreServ)
        else
            table.insert(without, coreServ)
        end
    end
    if isContains then
        return with
    else
        return without
    end
end

local function GetFirstServerWith(marks, isRemark, isContains)
    
    assert(type(marks) == "table")
    assert(type(isRemark), "boolean")
    assert(type(isContains), "boolean")
    
    for _, coreServ in ipairs(GetAllServs()) do
        local coreState = coreServ:GetCoreStates()
        local mark = isRemark and coreState:GetRemark() or coreState:GetMark()
        local has = IsInTable(marks, mark)
        if isContains then
            if has then
                return coreServ
            end
        else
            if not has then
                return coreServ
            end
        end
    end
    return nil
end

-- utils
function u.GC(hideStats)
    local prev = collectgarbage("count")
    collectgarbage("collect")
    if hideStats ~= true then
        local cur = math.floor(collectgarbage("count"))
        local diff = math.floor(prev - cur)
        print("Mem stat: collected ", diff, " KiB current ", cur, " KiB")
    end
end

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

function u.GetServersWithMarks(marks)
    return GetServsByMarks(marks, false, true)
end

function u.GetServersWithRemarks(remarks)
    return GetServsByMarks(remarks, true, true)
end

function u.GetServersWithoutMarks(marks)
    return GetServsByMarks(marks, false, false)
end

function u.GetServersWithoutRemarks(remarks)
    return GetServsByMarks(remarks, true, false)
end

function u.GetFirstServerWithMarks(marks)
    return GetFirstServerWith(marks, false, true)
end

function u.GetFirstServerWithRemarks(remarks)
    return GetFirstServerWith(remarks, true, true)
end

function u.GetFirstServerWithoutMarks(marks)
    return GetFirstServerWith(marks, false, false)
end

function u.GetFirstServerWithoutRemarks(remarks)
    return GetFirstServerWith(remarks, false, false)
end

function u.GetFirstServerWithName(name)
    assert(type(name) == "string")
    for _, coreServ in ipairs(GetAllServs()) do
        local coreState = coreServ:GetCoreStates()
        if coreState:GetName() == name then
            return coreServ
        end
    end
    return nil
end

function u.GetFirstServerWithUid(uid)
    assert(type(uid) == "string")
    return Server:GetServerByUid(uid)
end

function u.GetServerByIndex(index)
    assert(type(index) == "number")
    return Server:GetServerByIndex(index)
end

function u.InvertServersSelection(coreServs)
    assert(type(coreServs) == "table")
    for k, coreServ in pairs(coreServs) do
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

function u.SelectAllTimeouted()
    for _, coreServ in ipairs(GetAllServs()) do
        local coreState = coreServ:GetCoreStates()
        local latency = coreState:GetSpeedTestResult()
        local isTimeout = latency == u.Timeout
        coreState:SetIsSelected(isTimeout)
    end
end

function u.SelectAll()
    for _, coreServ in ipairs(GetAllServs()) do
        local coreState = coreServ:GetCoreStates()
        coreState:SetIsSelected(true)
    end
end

function u.InvertSelection()
    for _, coreServ in ipairs(GetAllServs()) do
        local coreState = coreServ:GetCoreStates()
        local selected = coreState:IsSelected()
        coreState:SetIsSelected(not selected)
    end
end

function u.SelectNone()
    for _, coreServ in ipairs(GetAllServs()) do
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

function u.ToLuaTicks(cSharpTicks)
    return ToLuaTicks(cSharpTicks)
end

function u.ToLuaDate(cSharpTicks)
    local t = ToLuaTicks(cSharpTicks)
    return os.date('%Y-%m-%d %H:%M:%S', t)
end

-- 这样写有代码提示
function u.GetAllServers()
    return GetAllServs()
end

function u.GetAllWrappedServers()
    return GetAllWrappedServs()
end

return u