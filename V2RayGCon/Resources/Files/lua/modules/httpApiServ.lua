--[[

example:

local url = "http://localhost:4000/"

local haServ = require('lua.modules.httpApiServ').new()

local handlers = {
    ["hello"] = function() return "world" end, 
    ["goodbye"] = function() haServ:Close() return "bye" end,
}

local html = "<html><body><h1>index.html</h1></body></html>"
haServ:Create(url, html, handlers)

print("Waiting for connections ...")
haServ:Run()

--]]

local M = {}

local function CreateServer(self)
    
    local inbox = Sys:ApplyRandomMailBox()
    assert(inbox ~= nil, "Apply mail box failed!")
    
    local outbox = Sys:ApplyRandomMailBox()
    assert(outbox ~= nil, "Apply mail box failed!")
    
    self.inbox = inbox
    self.outbox = outbox
    self.outadd = outbox:GetAddress()
    
    local serv = Sys:CreateHttpServer(self.url, self.inbox, self.outbox)
    return serv
end

local function Response(self, title, text)
    self.outbox:Send(self.outadd, title, text)
end

local function HandlePost(self, title, cmd)
    local r = ""
    if self.handlers[cmd] == nil then
        r = "unknow cmd: " .. cmd
    else
        r = self.handlers[cmd]()
    end
    
    Response(self, title, r)
end

local function HandleOneConn(self)
    
    local mail = self.inbox:Wait()
    
    if mail == nil then
        return false
    end
    
    local code = mail:GetCode()
    local title = mail:GetTitle()
    if code ~= 1 and self.html ~= nil then
        Response(self, title, self.html)
    else
        local cmd = mail:GetContent()
        HandlePost(self, title, cmd)
    end
    
    return true
end

function M:Create(url, html, handlers)
    
    assert(type(url) == "string", "Param url should be string")
    assert(type(handlers) == "table", "Param handlers should be function table.")
    assert(type(html) == "string", "Param index should be string.")
    
    self.url = url
    self.html = html
    self.handlers = handlers
    self.serv = CreateServer(self)
    
end

function M:Close()
    self.inbox:Close()
end

function M:Run()
    self.serv:Start()
    repeat
        local ok = HandleOneConn(self)
    until not ok
    self.serv:Stop()
end

function M.new()
    local o = {}
    setmetatable(o, {__index = M})
    return o
end

return M