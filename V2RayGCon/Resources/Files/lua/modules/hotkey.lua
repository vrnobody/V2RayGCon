local hotkey = {}

function hotkey:Reg(keyName, fn, hasAlt, hasCtrl, hasShift)
    local evCode = table.length(self.handles) + 1
    local handle = Sys:RegisterHotKey(self.mailbox, evCode, keyName, hasAlt, hasCtrl, hasShift)
    if handle ~= nil then
        table.insert(self.fns, fn)
        table.insert(self.handles, handle)
        return true
    end
    return false
end

function hotkey:ClearEvents()
    self.mailbox:Clear()
end

function hotkey:Destroy()
    for idx, handle in ipairs(self.handles) do
        Sys:UnregisterHotKey(self.mailbox, handle)
    end
    self:ClearEvents()
    self.mailbox:Close()
end

function hotkey:ExecMail(mail)
    if mail ~= nil then
        local evCode = mail:GetCode()
        self.fns[evCode]()
        return true
    end
    return false
end

function hotkey:Check()
    local mail = self.mailbox:Check()
    return self:ExecMail(mail)
end

function hotkey:Wait(milSec)
    local mail = nil
    if milSec == nil then
        mail = self.mailbox:Wait()
    else
        mail = self.mailbox:Wait(milSec)
    end
    return self:ExecMail(mail)
end

function hotkey.new()
    
    local mailbox = Sys:ApplyRandomMailBox()
    assert(mailbox ~= nil, "apply mailbox fail!")
    
    local o = {
        mailbox = mailbox,
        fns = {},
        handles = {},
    }
    
    setmetatable(o, {__index = hotkey})
    return o
end

return hotkey