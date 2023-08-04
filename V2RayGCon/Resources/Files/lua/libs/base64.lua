--[[
用法：

local b64 = require('lua.libs.base64')

function add(a, b)
    return a + b
end

local s = b64.encode_func(add)
print("ser:", s)

local f = b64.decode_func(s)
print("type(f):", type(f))

local a = 1
local b = 2
print(a, "+", b, "=", f(a, b))

--]]

local b='ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/' -- You will need this for encoding/decoding

local M = {}

-- encoding
local function encode(data)
    return ((data:gsub('.', function(x) 
        local r,b='',x:byte()
        for i=8,1,-1 do r=r..(b%2^i-b%2^(i-1)>0 and '1' or '0') end
        return r;
    end)..'0000'):gsub('%d%d%d?%d?%d?%d?', function(x)
        if (#x < 6) then return '' end
        local c=0
        for i=1,6 do c=c+(x:sub(i,i)=='1' and 2^(6-i) or 0) end
        return b:sub(c+1,c+1)
    end)..({ '', '==', '=' })[#data%3+1])
end

-- decoding
local function decode(data)
    data = string.gsub(data, '[^'..b..'=]', '')
    return (data:gsub('.', function(x)
        if (x == '=') then return '' end
        local r,f='',(b:find(x)-1)
        for i=6,1,-1 do r=r..(f%2^i-f%2^(i-1)>0 and '1' or '0') end
        return r;
    end):gsub('%d%d%d?%d?%d?%d?%d?%d?', function(x)
        if (#x ~= 8) then return '' end
        local c=0
        for i=1,8 do c=c+(x:sub(i,i)=='1' and 2^(8-i) or 0) end
            return string.char(c)
    end))
end

function M.encode_func(func)
    local bytes = string.dump(func)
    return encode(bytes)
end

function M.decode_func(str)
    local bytes = decode(str)
    return load(bytes)
end

function M.encode(data)
    return encode(data)
end

function M.decode(data)
    return decode(data)
end

return M
