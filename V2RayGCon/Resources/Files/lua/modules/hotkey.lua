local function Reg(self, keyName, fn, hasAlt, hasCtrl, hasShift)
    local evCode = table.length(self.handles) + 1
    local handle = Sys:RegisterHotKey(self.mailbox, evCode, keyName, hasAlt, hasCtrl, hasShift)
    if handle ~= nil then
        table.insert(self.fns, fn)
        table.insert(self.handles, handle)
        return true
    end
    return false
end

local function ClearEvents(self)
    self.mailbox:Clear()
end

local function Destroy(self)
    for idx, handle in ipairs(self.handles) do
        Sys:UnregisterHotKey(self.mailbox, handle)
    end
    ClearEvents(self)
    self.mailbox:Close()
end

local function ExecMail(self, mail)
    if mail ~= nil then
        local evCode = mail:GetCode()
        self.fns[evCode]()
        return true
    end
    return false
end

local function Check(self)
    local mail = self.mailbox:Check()
    return ExecMail(self, mail)
end

local function Wait(self, milSec)
    local mail = nil
    if milSec == nil then
        while true do
            mail = self.mailbox:Wait(5000)
            if mail ~= nil then
                return ExecMail(self, mail)
            end
            if self.mailbox:IsCompleted() then
                return false
            end
        end
    else
        mail = self.mailbox:Wait(milSec)
    end
    return ExecMail(self, mail)
end

local function Create()
    local mailbox = Sys:ApplyRandomMailBox()
    assert(mailbox ~= nil, "apply mailbox fail!")
    
    local hotkey = {}
    
    hotkey.mailbox = mailbox
    
    hotkey.fns = {}
    hotkey.handles = {}
    
    hotkey.Reg = Reg
    hotkey.ClearEvents = ClearEvents
    hotkey.Destroy = Destroy
    hotkey.Check = Check
    hotkey.Wait = Wait
        
    return hotkey
end

return Create