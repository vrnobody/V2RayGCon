--[[

example:

local url = "http://localhost:4000/"

local haServ = require('lua.modules.httpServ').new()

local handler = function(req)
    print("req: ", req)
    local resp = "ok"
    return resp
end

-- source can be file path or folder path or string of HTML
local source = "<html><body><h1>index.html</h1></body></html>"
haServ:Create(url, source, handler)

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
    
    local serv = Sys:CreateHttpServer(self.url, self.inbox, self.outbox, self.source)
    return serv
end

local function Response(self, title, text)
    self.outbox:Send(self.outadd, title, text)
end

local function HandlePost(self, title, req)
    local r = self.handle(req)
    Response(self, title, r)
end

local function HandleOneConn(self)
    
    local mail = self.inbox:Wait()
    
    if mail == nil then
        return false
    end
    
    local code = mail:GetCode()
    local title = mail:GetTitle()
    if code ~= 1 and self.source ~= nil then
        Response(self, title, self.source)
    else
        local req = mail:GetContent()
        HandlePost(self, title, req)
    end
    
    return true
end

function M:Create(url, source, handler)
    
    assert(type(url) == "string", "Param url should be string")
    assert(type(source) == "string", "Param source should be string.")
    assert(string.len(source) > 0, "Length of param source should greater then zero.")
    
    if handler == nil or type(handler) ~= "function" then
        handler = function() end
    end
    
    self.url = url
    self.source = source
    self.handle = handler
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
