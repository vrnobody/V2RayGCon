-- settings
local DefaultLogFilename = "LuaLog.txt"

local Logger = {}

-- helper function
local function WriteLine(filename, content)
	local file = io.open(filename, "a")
    file:write(tostring(content) .. "\n")
    file:close()
end


function Logger:Warn(text)
	self:Log("Warn", text)
end

function Logger:Debug(text)
	self:Log("Debug", text)
end

function Logger:Error(text)
	self:Log("Error", text)
end

function Logger:Info(text)
	self:Log("Info", text)
end

function Logger:Log(prefix, content)
	if prefix == nil then
		prefix = ""
	else
		prefix = " [" .. prefix .. "] "
	end
	local timestamp = os.date("[%Y-%m-%d %X]")
	local line = timestamp .. prefix .. tostring(content)
	WriteLine(self.filename, line)
end

function Logger.new(filename)
    
	local o = {
        filename = filename or DefaultLogFilename
    }
    
    setmetatable(o, {__index = Logger})
	return o
end

return Logger