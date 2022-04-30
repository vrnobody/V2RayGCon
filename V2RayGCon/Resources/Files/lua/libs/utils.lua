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
local function AllServs()
    return Each(Server:GetAllServers())
end

local function FirstServerOf(servers)
    assert(type(servers) == "table")
    if #servers > 0 then
        return servers[1]
    end
    return nil
end

local function ToNumber(str)
    if string.isempty(str) then
        return 0
    end
    return tonumber(str)
end

local function ToLuaTicks(cSharpTicks)
    local t = ToNumber(cSharpTicks)
    return math.floor(t / 10000000 - 62135596800)
end

local function IsInTable(haystack, needle)
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

--- Check if a file or directory exists in this path
local function Exists(file)
   local ok, err, code = os.rename(file, file)
   if not ok then
      if code == 13 then
         -- Permission denied, but it exists
         return true
      end
   end
   return ok, err
end

local function GetServsByMarks(marks, isRemark, isContainsMark)
    
    assert(type(marks) == "table")
    assert(type(isContainsMark), "boolean")
    assert(type(isRemark), "boolean")
    
    local hasMark = {}
    local noMark = {}
    for coreServ in AllServs() do
        local coreState = coreServ:GetCoreStates()
        local mark = isRemark and coreState:GetRemark() or coreState:GetMark()
        if IsInTable(marks, mark) then
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

-- utils

function u.Exists(file)
    return Exists(file)
end

function u.IsDir(path)
   return Exists(path.."/")
end

function u.GC()
    local prev = collectgarbage("count")
    collectgarbage("collect")
    local cur = math.floor(collectgarbage("count"))
    local diff = math.floor(prev - cur)
    print("Mem stat: collected ", diff, " KiB current ", cur, " KiB")
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
    local servs = GetServsByMarks(marks, false, true)
    return FirstServerOf(servs)
end

function u.GetFirstServerWithRemarks(remarks)
    local servs = GetServsByMarks(remarks, true, true)
    return FirstServerOf(servs)
end

function u.GetFirstServerWithoutMarks(marks)
    local servs = GetServsByMarks(marks, false, false)
    return FirstServerOf(servs)
end

function u.GetFirstServerWithoutRemarks(remarks)
    local servs = GetServsByMarks(remarks, true, false)
    return FirstServerOf(servs)
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

function u.ToLuaTicks(cSharpTicks)
    return ToLuaTicks(cSharpTicks)
end

function u.ToLuaDate(cSharpTicks)
    local t = ToLuaTicks(cSharpTicks)
    return os.date('%Y-%m-%d %H:%M:%S', t)
end

return u