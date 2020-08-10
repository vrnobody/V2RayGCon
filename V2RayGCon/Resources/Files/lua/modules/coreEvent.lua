local coreEv = {}

function coreEv:Reg(coreServ, fn, isEvStart)
    local evCode = table.length(self.handles) + 1
    local handle = isEvStart 
        and Sys:RegisterCoreStartEvent(coreServ, self.mailbox, evCode)
        or Sys:RegisterCoreStopEvent(coreServ, self.mailbox, evCode)
        
    if handle ~= nil then
        table.insert(self.evTypes, isEvStart)
        table.insert(self.fns, fn)
        table.insert(self.handles, handle)
        return true
    end
    return false
end

function coreEv:ClearEvents()
    self.mailbox:Clear()
end

function coreEv:Destroy()
    for idx, handle in ipairs(self.handles) do
        if self.evTypes[idx] then
            Sys:UnregisterCoreStartEvent(self.mailbox, handle)
        else
            Sys:UnregisterCoreStopEvent(self.mailbox, handle)
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