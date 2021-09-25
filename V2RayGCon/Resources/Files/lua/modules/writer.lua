local Writer = {}

-- settings
local DefaultFilename = "LuaData.txt"

local function ToString(content)
    if type(content) == "string" then
        return content
    end
    return tostring(content)
end

-- helper functions
local function WriteToFile(filename, content)
	local file = io.open(filename, "a")
    file:write(ToString(content) .. "\n")
    file:close()
end

local function ClearFile(filename)
	local file = io.open(filename, "w+")
	file:close()
end

function Writer:WriteAllText(content)
    local file = io.open(self.filename, "w+")
    file:write(ToString(content))
    file:close()
end

function Writer:WriteLine(content)
	WriteToFile(self.filename, content)
end

function Writer:Clear()
	ClearFile(self.filename)
end

function Writer.new(filename)
    local o = {
        filename = filename or DefaultFilename
    }
	setmetatable(o, {__index = Writer})
	return o
end

return Writer