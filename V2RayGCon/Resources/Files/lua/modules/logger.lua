-- settings
local DefaultLogFilename = "LuaLog.txt"

local Logger = {}

-- helper function
local function WriteLine(filename, content)
	local file = io.open(filename, "a")
    file:write(tostring(content) .. "\n")
    file:close()
end

Logger.logLevels = {
    ["Debug"] = 8,
    ["Info"] = 4,
    ["Warn"] = 2,
    ["Error"] = 1,
}

function Logger:Warn(text)
    if self.logLevel >= Logger.logLevels["Warn"] then
        self:Log("Warn", text)
    end
end

function Logger:Debug(text)
    if self.logLevel >= Logger.logLevels["Debug"] then
        self:Log("Debug", text)
    end
end

function Logger:Error(text)
    if self.logLevel >= Logger.logLevels["Error"] then
        self:Log("Error", text)
    end
end

function Logger:Info(text)
    if self.logLevel >= Logger.logLevels["Info"] then
        self:Log("Info", text)
    end
end

function Logger:Log(prefix, content)
	if prefix == nil then
		prefix = ""
	else
		prefix = " [" .. prefix .. "] "
	end
	local timestamp = os.date("[%Y-%m-%d %X]")
	local line = timestamp .. prefix .. tostring(content)
    if string.isempty(self.filename) then
        print(line)
    else
        WriteLine(self.filename, line)
    end
end

function Logger:SetLogLevel(logLevel)
    if logLevel == nil or not table.contains(Logger.logLevels, logLevel) then
        print("unsupported log level: ", logLevel)
        logLevel = Logger.logLevels["Debug"]
    end
    self.logLevel = logLevel
end

function Logger.new(filename, logLevel)
	local o = {
        filename = filename,
        logLevel = Logger.logLevels["Debug"]
    }
    
    setmetatable(o, {__index = Logger})
    o:SetLogLevel(logLevel)
	return o
end

return Logger