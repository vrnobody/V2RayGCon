local utils = require "lua.libs.utils"
local HotKey = require "lua.modules.hotkey"

local hkMgr = HotKey.new()

local function SayHello()
    Misc:Alert("hello")
end

hkMgr:Reg("D5", SayHello, true, true, false)
hkMgr:ClearEvents()


while not Signal:Stop() do
    hkMgr:Wait(1000)
end


