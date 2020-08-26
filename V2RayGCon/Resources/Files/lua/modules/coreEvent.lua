local coreEv = {}

local GLOBAL_EV = 1
local CORE_EV = 2

local function RegEvHandler(self, coreServ, fn, evType)
    
    local evCode = table.length(self.handles) + 1
    
    local handle
    local et
    
    if coreServ == nil then
        et = GLOBAL_EV
        handle = Sys:RegisterGlobalEvent(self.mailbox, evType, evCode)
    else
        et = CORE_EV
        handle = Sys:RegisterCoreEvent(coreServ, self.mailbox, evType, evCode)
    end
    
    if handle ~= nil then
        table.insert(self.evTypes, et)
        table.insert(self.fns, fn)
        table.insert(self.handles, handle)
        return true
    end
    return false
end

--[[
function coreEv:RegEvClosing(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Sys.CoreEvClosing)
end
--]]

function coreEv:RegGlobalEvStart(handler)
    return RegEvHandler(self, nil, handler, Sys.CoreEvStart)
end

function coreEv:RegGlobalEvStop(handler)
    return RegEvHandler(self, nil, handler, Sys.CoreEvStop)
end

function coreEv:RegEvPropertyChanged(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Sys.CoreEvPropertyChanged)
end

function coreEv:RegEvStart(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Sys.CoreEvStart)
end

function coreEv:RegEvStop(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Sys.CoreEvStop)
end

function coreEv:ClearEvents()
    self.mailbox:Clear()
end

function coreEv:Destroy()
    for idx, handle in ipairs(self.handles) do
        local et = self.evTypes[idx]
        if et == GLOBAL_EV then
            Sys:UnregisterGlobalEvent(self.mailbox, handle)
        elseif et == CORE_EV then
            Sys:UnregisterCoreEvent(self.mailbox, handle)
        end
    end
    self:ClearEvents()
    self.mailbox:Close()
end

function coreEv:ExecMail(mail)
    if mail ~= nil then
        local evCode = mail:GetCode()
        local et = self.evTypes[evCode]
        if et == GLOBAL_EV then
            local uid = mail:GetContent()
            self.fns[evCode](uid)
        elseif et == CORE_EV then
            self.fns[evCode]()
        end
        return true
    end
    return false
end

function coreEv:Check()
    local mail = self.mailbox:Check()
    return self:ExecMail(mail)
end

function coreEv:Wait(milSec)
    local mail = nil
    if milSec == nil then
        mail = self.mailbox:Wait()
    else
        mail = self.mailbox:Wait(milSec)
    end
    return self:ExecMail(mail)
end

function coreEv.new()
    
    local mailbox = Sys:ApplyRandomMailBox()
    assert(mailbox ~= nil, "apply mailbox fail!")
    
    local o = {
        mailbox = mailbox,
        evTypes = {},
        fns = {},
        handles = {},
    }
    
    setmetatable(o, {__index = coreEv})
    return o
end

return coreEv