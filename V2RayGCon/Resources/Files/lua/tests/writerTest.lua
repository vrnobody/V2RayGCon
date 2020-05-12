local Writer = require "lua.modules.writer"

local wt = Writer.new("w.txt")
local content = "hello"
wt:WriteLine(content)
wt:WriteLine(content)
wt:Clear()
wt:WriteLine(nil)
wt:WriteLine(content)
