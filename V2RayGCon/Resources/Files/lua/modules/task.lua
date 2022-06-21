--[[

本模块参（抄）考（袭）自LuatOS，用法样例：

local Task = require('lua.modules.task').new()

function task1()
    for i = 1, 10 do
        print("Task 1: ", i)
        Task:Wait(1000)
    end
    Task:Stop()
end

function task2()
    for i = 1, 10 do
        print("Task 2: ", i)
        Task:Wait(500)
    end
end

Task:Init(task1)
Task:Init(task2)
Task:Run()
print("Done!")

]]

-- TaskID最大值
local TASK_TIMER_ID_MAX = 0x1FFFFF
-- 任务定时器id
local taskTimerId = 0
-- 定时器id表
local timerPool = {}
local taskTimerPool = {}

local M = {}

function M:Init(fun, ...)
    local co = coroutine.create(fun)
    coroutine.resume(co, ...)
    return co
end

function M:Wait(timeout)
    -- 选一个未使用的定时器ID给该任务线程
    while true do
        if taskTimerId >= TASK_TIMER_ID_MAX - 1 then
            taskTimerId = 0
        else
            taskTimerId = taskTimerId + 1
        end
        if taskTimerPool[taskTimerId] == nil then
            break
        end
    end
    local timerid = taskTimerId
    taskTimerPool[coroutine.running()] = timerid
    timerPool[timerid] = coroutine.running()
    -- 调用定时器
    Sys:SetTimeout(self.mailbox, timeout, timerid)
    -- 挂起调用的任务线程
    coroutine.yield()
    taskTimerPool[coroutine.running()] = nil
    timerPool[timerid] = nil
end

function M:Run()
    repeat
        local mail = self.mailbox:Wait()
        if mail ~= nil then
            local param = mail:GetCode()
            if timerPool[param] then
                local taskId = timerPool[param]
                timerPool[param] = nil
                if taskTimerPool[taskId] == param then
                    taskTimerPool[taskId] = nil
                    coroutine.resume(taskId)
                end
            end
        end
    until mail == nil
end

function M:Stop()
    self.mailbox:Close()
end

function M.new()
    local mailbox = Sys:ApplyRandomMailBox()
    local o = {
        mailbox = mailbox
    }
    setmetatable(o, {__index = M})
    return o
end

return M