-- settings
local Logger = {}

Logger.logLevels = {
    ["Debug"] = 8,
    ["Info"] = 4,
    ["Warn"] = 2,
    ["Error"] = 1,
}

function Logger:Warn(...)
    if self.logLevel >= Logger.logLevels["Warn"] then
        self:Log("Warn", ...)
    end
end

function Logger:Debug(...)
    if self.logLevel >= Logger.logLevels["Debug"] then
        self:Log("Debug", ...)
    end
end

function Logger:Error(...)
    if self.logLevel >= Logger.logLevels["Error"] then
        self:Log("Error", ...)
    end
end

function Logger:Info(...)
    if self.logLevel >= Logger.logLevels["Info"] then
        self:Log("Info", ...)
    end
end

function Logger:Log(prefix, ...)
	if prefix == nil then
		prefix = ""
	else
		prefix = "[" .. prefix .. "]"
	end
	local timestamp = os.date("[%Y-%m-%d %X]")
    local t = {}
    for _, v in pairs({timestamp, prefix, ...}) do
        table.insert(t, tostring(v))
    end
	local line = table.concat(t, " ")
    local file = self.filename
    if not string.isempty(file) then
        std.Sys:WriteLine(file, line)
    end
    if self.quiet ~= true then
        print(line)
    end
end

function Logger:SetLogLevel(logLevel)
    if logLevel == nil or not table.contains(Logger.logLevels, logLevel) then
        local level = "Info"
        logLevel = Logger.logLevels[level]
        if string.isempty(self.filename) then
            print("log level:", level)
        else
            print("log level:", level, ", file:", self.filename)
        end
    end
    self.logLevel = logLevel
end

function Logger.new(filename, quiet, level)
    
	local o = {
        filename = filename,
        logLevel = level or Logger.logLevels.Info,
        quiet = quiet,
    }
    
    setmetatable(o, {__index = Logger})
    o:SetLogLevel(level)
	return o
end

return Logger