local coreEv = {}

local Events = {
    OnCoreStart = 1,
    
    -- not implement yet
    -- OnCoreClosing = 2,
    
    OnCoreStop = 3,
}

local function BindToCoreEvent(coreServ, mailbox, evCode, evType)
    if evType == Events.OnCoreStart then
        return Sys:RegisterCoreStartEvent(coreServ, mailbox, evCode)
    elseif evType == Events.OnCoreStop then
        return Sys:RegisterCoreStopEvent(coreServ, mailbox, evCode)
    end
    return nil
end

local function RegEvHandler(self, coreServ, fn, evType)
    local evCode = table.length(self.handles) + 1
    local handle = BindToCoreEvent(coreServ, self.mailbox, evCode, evType)
        
    if handle ~= nil then
        table.insert(self.evTypes, evType)
        table.insert(self.fns, fn)
        table.insert(self.handles, handle)
        return true
    end
    return false
end

function coreEv:RegStartEv(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Events.OnCoreStart)
end

function coreEv:RegStopEv(coreServ, handler)
    return RegEvHandler(self, coreServ, handler, Events.OnCoreStop)
end

function coreEv:ClearEvents()
    self.mailbox:Clear()
end

function coreEv:Destroy()
    for idx, handle in ipairs(self.handles) do
        local evType = self.evTypes[idx]
        if evType == Events.OnCoreStart then
            Sys:UnregisterCoreStartEvent(self.mailbox, handle)
        elseif evType == Events.OnCoreStop then
            Sys:UnregisterCoreStopEvent(self.mailbox, handle)
        else
            assert(false, "unsupported event type: " .. tostring(evType))
        end
    end
    self:ClearEvents()
    self.mailbox:Close()
end

function coreEv:ExecMail(mail)
    if mail ~= nil then
        local evCode = mail:GetCode()
        self.fns[evCode]()
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