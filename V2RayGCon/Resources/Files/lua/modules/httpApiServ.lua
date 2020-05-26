--[[

example:

local url = "http://localhost:4000/"

local handlers = {
    ["hello"] = function() return "world" end, 
}

local html = "<html><body><h1>index.html</h1></body></html>"

local hapi = require('lua.modules.httpApiServ').new()

if hapi:Run(url, html, handlers) then
    print("Waiting for connections ...")
    hapi:HandleConnection()
else
    print("Server fails to start!")
end

--]]

local M = {}

function M:Run(url, html, handlers)
    
    assert(type(handlers) == "table", "Param handlers should be function table.")
    assert(type(html) == "string", "Param index should be string.")
        
    local inbox = Sys:ApplyRandomMailBox()
    assert(inbox ~= nil, "Apply mail box failed!")
    
    local outbox = Sys:ApplyRandomMailBox()
    assert(outbox ~= nil, "Apply mail box failed!")
    
    self.url = url
    self.html = html
    self.handlers = handlers
    
    self.inbox = inbox
    self.outbox = outbox
    self.outadd = outbox:GetAddress()
    
    local ok = Sys:CreateHttpServer(self.url, self.inbox, self.outbox)
    return ok
end

function M:HandlePost(title, cmd)
    local r = ""
    if self.handlers[cmd] == nil then
        r = "unknow cmd: " .. cmd
    else
        r = self.handlers[cmd]()
    end
    
    self.outbox:Send(self.outadd, title, r)
end

function M:HandleConnection()
    repeat
        local mail = self.inbox:Wait()
        if mail ~= nil then
            local code = mail:GetCode()
            local title = mail:GetTitle()
            if code ~= 1 and self.html ~= nil then
                self.outbox:Send(self.outadd, title, self.html)
            else
                local cmd = mail:GetContent()
                self:HandlePost(title, cmd)
            end
        end
    until mail == nil
end

function M.new()
    local o = {}
    
    setmetatable(o, {__index = M})
    
    return o
end

return M