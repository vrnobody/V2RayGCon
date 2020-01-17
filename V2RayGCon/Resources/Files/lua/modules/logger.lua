-- settings
local DefaultLogFilename = "LuaLog.txt"

-- helper function
local function WriteLine(filename, content)
	local file = io.open(filename, "a")
    file:write(content .. "\n")
    file:close()
end

-- class function
local function Init(self, filename)
	if filename == nil then
		filename = DefaultLogFilename
	end
	self.filename = filename
end

local function Warn(self, text)
	self:Log("Warn", text)
end

local function Debug(self, text)
	self:Log("Debug", text)
end

local function Error(self, text)
	self:Log("Error", text)
end

local function Info(self, text)
	self:Log("Info", text)
end

local function Log(self, prefix, content)
	if prefix == nil then
		prefix = ""
	else
		prefix = " [" .. prefix .. "] "
	end
	local timestamp = os.date("[%Y-%m-%d %X]")
	local line = timestamp .. prefix .. content
	WriteLine(self.filename, line)
end

local function Create(filename)
	local Logger = {}
	Init(Logger, filename)
	
	Logger.Debug = Debug
	Logger.Error = Error
	Logger.Info = Info
	Logger.Log = Log
	Logger.Warn = Warn
	
	return Logger
end

return Create