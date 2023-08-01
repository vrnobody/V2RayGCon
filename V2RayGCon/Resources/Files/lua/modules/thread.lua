--[[

这个模块把函数序列化到一个新线程中执行。

样例：

local thread = require('lua.modules.thread')

-- 因为下面这个函数将在新线程中执行
-- 所以不可以引用函数外的变量、模块等等
-- 也不可以递归调用自己
local function counter(name, value)
    local c = value
    repeat
        print(name .. ":", c)
        c = c + 1
        Misc:Sleep(1000)
    until Signal:Stop()
    return c
end

-- 创建一个匿名的线程
local t1 = thread.new(counter)

-- 创建一个带名字的线程，print()时会加上这个前缀
local t2 = thread.new(counter, "T2")

-- 因为跨线程需要序列化，所以参数只可以是简单类型
-- 对应 counter(name, value)
t1:Start("#1", 100)
t2:Start("#2", 200)

Misc:Sleep(5000)

-- 发送停止信号
thread.StopAll(t1, t2)

-- 等待停止并接收返回值
print("result1:", t1:WaitResult())
print("result2:", t2:WaitResult())

--]]

local base64 = require('lua.libs.base64')
local json = require('lua.libs.json')

local M = {}

local tpl = [[
local base64 = require('lua.libs.base64')
local json = require('lua.libs.json')

local sp = "{param}"
local sf = "{function}"

local ps = json.decode(base64.decode(sp))
local f = base64.decode_func(sf)

return f(table.unpack(ps))
]]

local function replace_params(script, ...)
    local sp = json.encode({...})

    -- 引号转义问题
    local b64 = base64.encode(sp)
    
    return string.gsub(script, "{param}", b64)
end

local function replace_func(script, func)
    local sf = base64.encode_func(func)
    return string.gsub(script, "{function}", sf)
end

function M:Start(...)
    local script = replace_params(self.script, ...)
    assert(type(script) == "string", "parse params failed")
    
    -- for debugging
    -- print(script)
    
    self.vmh = Sys:LuaVmCreate(self.tag)
    return Sys:LuaVmRun(self.vmh, script)
end

function M:GetResult()
    local j = Sys:LuaVmGetResult(self.vmh)
    local r = json.decode(j)
    return table.unpack(r)
end
    
function M:IsRunning()
    return Sys:LuaVmIsRunning(self.vmh)
end

function M:WaitResult()
    self:Wait()
    return self:GetResult()
end

function M:Abort()
    Sys:LuaVmAbort(self.vmh)
end

function M:Stop()
    Sys:LuaVmStop(self.vmh)
end

function M:Wait(ms)
    ms = ms and ms or -1
    Sys:LuaVmWait(self.vmh, ms)
end

local function StopAll(...)
    for k, v in pairs({...}) do
        v:Stop()
    end
end

-- 只可以用于不用传参数的线程
local function StartAll(...)
    for k, v in pairs({...}) do
        v:Start()
    end
end

local function WaitAll(...)
    for k, v in pairs({...}) do
        v:Wait()
    end
end

M.StopAll = StopAll
M.WaitAll = WaitAll

function M.new(func, tag)
    
    assert(type(func), "function", "func must be function")
    assert(tag == nil or type(tag), "string", "tag must be string or nil")
    
    local script = replace_func(tpl, func)
    assert(type(script) == "string", "parse function failed")
    local o = {
        tag = tag,
        script = script,
        vmh = nil,
    }
    setmetatable(o, {__index = M})
	return o
end

return M
