-- settings
local DefaultFilename = "LuaData.txt"

-- helper functions
local function WriteToFile(filename, content)
	local file = io.open(filename, "a")
    file:write(content .. "\n")
    file:close()
end

local function ClearFile(filename)
	local file = io.open(filename, "w+")
	file:close()
end

-- Writer
local function Init(self, filename)
	if filename == nil then
		filename = DefaultFilename
	end
	self.filename = filename
end

local function WriteLine(self, content)
	WriteToFile(self.filename, content)
end

local function Clear(self)
	ClearFile(self.filename)
end

local function Create(filename)
	local Writer = {}
	Init(Writer, filename)
	
	Writer.WriteLine = WriteLine
	Writer.Clear = Clear
	
	return Writer
end

return Create