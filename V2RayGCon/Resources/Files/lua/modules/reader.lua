local Reader = {}

-- https://stackoverflow.com/questions/11201262/how-to-read-data-from-a-file-in-lua
function Reader:IsFileExist(file)
  local f = io.open(file, "rb")
  if f then f:close() end
  return f ~= nil
end

-- https://stackoverflow.com/questions/10386672/reading-whole-files-in-lua
function Reader:ReadAllText()
    local f = assert(io.open(self.filename, "rb"))
    local content = f:read("*all")
    f:close()
    return content
end

-- https://stackoverflow.com/questions/15079914/lua-fastest-way-to-read-data
function Reader:ReadAllLines()
    local lines = self:GetLinesIter()
    local t = {}
    for line in lines do
       t[#t+1] = line
    end
    return t
end

-- for line in lines
function Reader:GetIter()
    return assert(io.lines(self.filename))
end

function Reader.new(filename)
    local o = {filename = filename}
	setmetatable(o, {__index = Reader})
	return o
end

return Reader