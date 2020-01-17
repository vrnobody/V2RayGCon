-- Reader
local function Init(self, filename)
	self.filename = filename
end

-- https://stackoverflow.com/questions/11201262/how-to-read-data-from-a-file-in-lua
local function IsFileExist(self, file)
  local f = io.open(file, "rb")
  if f then f:close() end
  return f ~= nil
end

-- https://stackoverflow.com/questions/10386672/reading-whole-files-in-lua
local function ReadAllText(self)
    local f = assert(io.open(self.filename, "rb"))
    local content = f:read("*all")
    f:close()
    return content
end

-- https://stackoverflow.com/questions/15079914/lua-fastest-way-to-read-data
local function ReadAllLines(self)
    local lines = assert(io.lines(self.filename))
    local t = {}
    for line in lines do
       t[#t+1] = line
    end
    return t
end

local function Create(filename)
	local Reader = {}
	Init(Reader, filename)
	
	Reader.ReadAllText = ReadAllText
	Reader.ReadAllLines = ReadAllLines
	Reader.IsFileExist = IsFileExist
	
	return Reader
end

return Create