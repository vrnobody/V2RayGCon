local Reader = require "lua.modules.reader"

local rd = Reader.new("log.txt")

print( rd:IsFileExist("log.txt") )
print( rd:ReadAllLines() )
print( rd:ReadAllText() )
