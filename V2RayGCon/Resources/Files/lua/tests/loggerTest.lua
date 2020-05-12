local Logger = require "lua.modules.logger"

local log = Logger.new("log.txt")

local text = "world!"
local prefix = "hello, "
log:Debug(text)
log:Error(text)
log:Info(nil)
log:Log(prefix, nil)
log:Warn(text)